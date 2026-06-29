using Ardalis.Result;
using FluentValidation;
using FluxStore.Api.Infrastructure.Persistence;
using FluxStore.Api.Infrastructure.Services;
using FluxStore.Api.Shared.Abstractions;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Models;
using FluxStore.Api.Shared.Settings;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FluxStore.Api.Features.Authentication.Login
{
    public class RequestHandler : IRequestHandler<LoginRequest, Result<LoginResponse>>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenService _tokenService;
        private readonly IDateTime _dateTime;
        private readonly TokenSettings _tokenSettings;

        public RequestHandler(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, TokenService tokenService, IDateTime dateTime, IOptions<TokenSettings> tokenSettings)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
            _dateTime = dateTime;
            _tokenSettings = tokenSettings.Value;
        }

        public async Task<Result<LoginResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
        {


            var applicationUser = await _userManager
                .Users.Include(X => X.RefreshTokens.Where(X => X.RevokedAtUTC == null && X.ExpiresAtUTC > _dateTime.UtcNow))
                .FirstOrDefaultAsync(u => u.Email == request.EmailAddress, cancellationToken);

            if (applicationUser == null)
                return Result<LoginResponse>.Unauthorized(IdentityErrors.InvalidCredentials.Code, IdentityErrors.InvalidCredentials.Description);

            var signInResult = await _signInManager.CheckPasswordSignInAsync(applicationUser, request.Password, lockoutOnFailure: false);

            if (!signInResult.Succeeded)
            {
                return Result<LoginResponse>.Unauthorized(IdentityErrors.InvalidCredentials.Code, IdentityErrors.InvalidCredentials.Description);
            }

            var roles = await _userManager.GetRolesAsync(applicationUser);

            bool hasActiveRefreshToken = applicationUser.RefreshTokens.Any(rt => rt.IsActive);

            if (hasActiveRefreshToken)
            {
                applicationUser.RefreshTokens.First().Revoke(_dateTime.UtcNow);
            }

            (string refreshToken, DateTime refreshTokenExpirationDate) = _tokenService.GenerateRefreshToken();

            (string accessToken, DateTime accessTokenExpirationDate) = _tokenService.GenerateAccessToken(applicationUser, roles);

            RefreshToken newRefreshToken = RefreshToken.Create(applicationUser.Id, refreshToken, refreshTokenExpirationDate);

            applicationUser.RefreshTokens.Add(newRefreshToken);

            var loginResponse = new LoginResponse
            {
                UserId = applicationUser.Id,
                FirstName = applicationUser.FirstName,
                LastName = applicationUser.LastName,
                UserName = applicationUser.UserName ?? string.Empty,
                Email = applicationUser.Email ?? string.Empty,
                Roles = roles.ToList(),
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return Result<LoginResponse>.Success(loginResponse);
        }
    }
}
