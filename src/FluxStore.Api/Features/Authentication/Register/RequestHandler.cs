using Ardalis.Result;
using FluxStore.Api.Domain.Enums;
using FluxStore.Api.Domain.UserAggregate;
using FluxStore.Api.Infrastructure.Persistence;
using FluxStore.Api.Infrastructure.Services;
using FluxStore.Api.Shared.Abstractions;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Extensions;
using FluxStore.Api.Shared.Models;
using FluxStore.Api.Shared.Settings;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace FluxStore.Api.Features.Authentication.Register
{
    public class RequestHandler : IRequestHandler<RegisterRequest, Result<RegisterResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;
        private readonly TokenSettings _tokenOptions;
        private readonly IDateTime _dateTime;

        public RequestHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole<Guid>> roleManager, ApplicationDbContext context, TokenService tokenService, IOptions<TokenSettings> tokenOptions, IDateTime dateTime)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _tokenService = tokenService;
            _tokenOptions = tokenOptions.Value;
            _dateTime = dateTime;
        }

        public async Task<Result<RegisterResponse>> Handle(RegisterRequest request, CancellationToken cancellationToken)
        {
            // Possible Errors 
            // EmailAlreadyExists (CONFLICT)
            // ALL PASSWORD TYPES ERRRORS (VALIDATION ERRORS)
            // Roles Related Errors (NOTFOUND,CONFLICT)



            if ((await _userManager.FindByEmailAsync(request.EmailAddress)) != null)

                return Result<RegisterResponse>.Conflict(IdentityErrors.EmailAlreadyExists.Code, IdentityErrors.EmailAlreadyExists.Description); ;


            ApplicationUser applicationUser = new ApplicationUser
            {
                FirstName = request.Name.Split(" ").FirstOrDefault() ?? "",
                LastName = request.Name.Split(" ").ElementAtOrDefault(1) ?? "",
                Email = request.EmailAddress,
                UserName = request.EmailAddress

            };

            var identityResult = await _userManager.CreateAsync(applicationUser, request.Password);


            if (!identityResult.Succeeded)

                return identityResult.ToResult();


            var addToRoleResult = await _userManager.AddToRoleAsync(applicationUser, ApplicationRoles.User);

            if (!addToRoleResult.Succeeded)

                return addToRoleResult.ToResult();


            var userRoles = new string[] { ApplicationRoles.User };


            (string refreshToken, DateTime refreshTokenExpirationDate) = _tokenService.GenerateRefreshToken();


            (string accessToken, DateTime accessTokenExpirationDate) = _tokenService.GenerateAccessToken(applicationUser, userRoles);


            RefreshToken newRefreshToken = RefreshToken.Create(applicationUser.Id, refreshToken, refreshTokenExpirationDate);


            var domainUser = User.Create(applicationUser.Id, applicationUser.FirstName, applicationUser.LastName, applicationUser.UserName, applicationUser.Email);


            applicationUser.RefreshTokens.Add(newRefreshToken);

            _context.DomainUsers.Add(domainUser);


            var registerResponse = new RegisterResponse
            {
                UserId = applicationUser.Id,
                FirstName = applicationUser.FirstName,
                LastName = applicationUser.LastName,
                UserName = applicationUser.UserName ?? string.Empty,
                Email = applicationUser.Email ?? string.Empty,
                Roles = userRoles.ToList(),
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };


            return Result<RegisterResponse>.Success(registerResponse);
        }
    }
}
