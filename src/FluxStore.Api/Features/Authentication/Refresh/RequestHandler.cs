using Ardalis.Result;
using FluxStore.Api.Infrastructure.Persistence;
using FluxStore.Api.Infrastructure.Services;
using FluxStore.Api.Shared.Abstractions;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Settings;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FluxStore.Api.Features.Authentication.Refresh
{
    public class RequestHandler : IRequestHandler<Endpoint, Result<RefreshTokenResponse>>
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

        public async Task<Result<RefreshTokenResponse>> Handle(Endpoint request, CancellationToken cancellationToken)
        {
            ApplicationUser? applicationUser = await _userManager
                 .Users
                 .Include(X => X.RefreshTokens)
                 .FirstOrDefaultAsync(ut => ut.RefreshTokens.Any(rt =>
                     rt.Token == request.RefreshToken &&
                     rt.ExpiresAtUTC > _dateTime.UtcNow &&
                     rt.RevokedAtUTC == null), cancellationToken);

            if (applicationUser == null)
            {
                return Result<RefreshTokenResponse>.Unauthorized(IdentityErrors.InvalidToken.Code, IdentityErrors.InvalidToken.Description);
            }

            var userRoles = await _userManager.GetRolesAsync(applicationUser);

            (string accessToken, DateTime accessTokenExpirationDate) = _tokenService.GenerateAccessToken(applicationUser, userRoles);

            var currentRefreshToken = applicationUser.RefreshTokens.First(rt => rt.Token == request.RefreshToken);

            var refreshTokenResponse = new RefreshTokenResponse(
                applicationUser.Id,
                applicationUser.Email!,
                applicationUser.UserName!,
                applicationUser.FirstName,
                applicationUser.LastName,
                userRoles,
                accessToken,
                currentRefreshToken.Token
            );

            return Result<RefreshTokenResponse>.Success(refreshTokenResponse);
        }
    }

}
