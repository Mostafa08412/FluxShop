using Ardalis.Result;
using FluentValidation;
using FluxStore.Api.Domain.Enums;
using FluxStore.Api.Domain.UserAggregate;
using FluxStore.Api.Infrastructure.Persistence;
using FluxStore.Api.Infrastructure.Services;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Models;
using FluxStore.Api.Shared.Settings;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace FluxStore.Api.Features.Authentication.GoogleLogin
{

    public class RequestHandler : IRequestHandler<GoogleLoginRequest, Result<GoogleLoginResponse>>
    {
        private readonly ILogger<RequestValidator> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenService _tokenService;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly ApplicationDbContext _applicationDbContext;

        public RequestHandler(ILogger<RequestValidator> logger, ApplicationDbContext applicationDbContext, IOptions<ExternalAuthenticationSettings> externalAthenticationSettings, UserManager<ApplicationUser> userManager, TokenService tokenService)
        {

            _userManager = userManager;
            _tokenService = tokenService;
            _externalAuthenticationSettings = externalAthenticationSettings.Value;
            _applicationDbContext = applicationDbContext;
            _logger = logger;
        }

        public async Task<Result<GoogleLoginResponse>> Handle(GoogleLoginRequest request, CancellationToken cancellationToken)
        {
            var userDataResult = await GetUserDataFromPayloadUsingIdToken(request.IdToken);

            if (!userDataResult.IsSuccess)
            {
                _logger.LogWarning("Failed to validate Google ID token. Errors: {Errors}", userDataResult.Errors);

                return Result<GoogleLoginResponse>.Error(new ErrorList(userDataResult.Errors));

            }

            var applicationUser = new ApplicationUser
            {
                Email = userDataResult.Value.Email,
                UserName = userDataResult.Value.Email,
                FirstName = userDataResult.Value.FirstName,
                LastName = userDataResult.Value.LastName,
                EmailConfirmed = true
            };

            var createUserResult = await _userManager.CreateAsync(applicationUser, Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)));

            (string refreshToken, DateTime refreshTokenExpirationDate) = _tokenService.GenerateRefreshToken();

            if (createUserResult.Succeeded)
            {
                // Hence, the user is new and we need to assign him the default role and create a domain user for him.

                var addToRoleResult = await _userManager.AddToRoleAsync(applicationUser, ApplicationRoles.User);


                var newUserRoles = new string[] { ApplicationRoles.User };


                (string accessToken, DateTime accessTokenExpirationDate) = _tokenService.GenerateAccessToken(applicationUser, newUserRoles);


                RefreshToken newRefreshToken = RefreshToken.Create(applicationUser.Id, refreshToken, refreshTokenExpirationDate);


                var domainUser = User.Create(applicationUser.Id, applicationUser.FirstName, applicationUser.LastName, applicationUser.UserName, applicationUser.Email);


                applicationUser.RefreshTokens.Add(newRefreshToken);


                _applicationDbContext.DomainUsers.Add(domainUser);


                return Result<GoogleLoginResponse>.Success(new GoogleLoginResponse
                {
                    UserId = applicationUser.Id,
                    FirstName = applicationUser.FirstName,
                    LastName = applicationUser.LastName,
                    UserName = applicationUser.UserName ?? string.Empty,
                    Email = applicationUser.Email ?? string.Empty,
                    Roles = newUserRoles,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });

            }

            ApplicationUser? user = await _userManager.FindByEmailAsync(userDataResult.Value.Email);

            if (user == null)
            {
                _logger.LogWarning("User with email {Email} not found in the database after successful Google ID token validation.", userDataResult.Value.Email);

                var errorList = new ErrorList(new string[] { IdentityErrors.InvalidGoogleIdToken.Code, IdentityErrors.InvalidGoogleIdToken.Description });

                return Result<GoogleLoginResponse>.Error(errorList);
            }

            var userRoles = await _userManager.GetRolesAsync(user!);

            foreach (var rt in user.RefreshTokens.Where(rt => rt.IsActive))
            {
                rt.Revoke(DateTime.UtcNow);
            }

            user.RefreshTokens.Add(RefreshToken.Create(user.Id, refreshToken, refreshTokenExpirationDate));

            await _userManager.UpdateAsync(user);

            return Result<GoogleLoginResponse>.Success(new GoogleLoginResponse
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = userRoles,
                AccessToken = _tokenService.GenerateAccessToken(user!, userRoles).Item1,
                RefreshToken = refreshToken
            });


        }

        private async Task<Result<GooglePayloadData>> GetUserDataFromPayloadUsingIdToken(string idToken)
        {
            GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { _externalAuthenticationSettings.Google.ClientId }
            };

            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                string email = payload.Email;
                string firstName = payload.GivenName;
                string lastName = payload.FamilyName;
                return Result<GooglePayloadData>.Success(new GooglePayloadData(email, firstName, lastName));

            }
            catch (Exception)
            {
                return Result<GooglePayloadData>.Error(new ErrorList(new string[] { IdentityErrors.InvalidGoogleIdToken.Code, IdentityErrors.InvalidGoogleIdToken.Description }));

            }


        }

        private record GooglePayloadData(string Email, string FirstName, string LastName);

    }
}
