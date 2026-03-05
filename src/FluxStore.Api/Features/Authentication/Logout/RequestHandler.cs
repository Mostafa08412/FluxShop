using Ardalis.Result;
using FluxStore.Api.Infrastructure.Persistence;
using FluxStore.Api.Infrastructure.Services;
using FluxStore.Api.Shared.Abstractions;
using FluxStore.Api.Shared.Errors;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FluxStore.Api.Features.Authentication.Logout
{
    public class RequestHandler : IRequestHandler<LogoutRequest, Result<Unit>>
    {

        private readonly ILogger<RequestHandler> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CurrentUser _currentUser;
        private readonly ApplicationDbContext _context;
        private readonly IDateTime _dateTime;

        public RequestHandler(ILogger<RequestHandler> logger, UserManager<ApplicationUser> userManager, CurrentUser currentUser, ApplicationDbContext context, IDateTime dateTime)
        {
            _logger = logger;
            _userManager = userManager;
            _currentUser = currentUser;
            _context = context;
            _dateTime = dateTime;
        }

        public async Task<Result<Unit>> Handle(LogoutRequest request, CancellationToken cancellationToken)
        {

            var user = await _userManager
                 .Users.Include(X => X.RefreshTokens.Where(X => X.RevokedAtUTC == null && X.ExpiresAtUTC > _dateTime.UtcNow))
                 .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, cancellationToken);

            if (user is null)
            {
                _logger.LogInformation("Logout failed: User with ID {UserId} not found.", _currentUser.UserId);
                return Result<Unit>.NotFound(IdentityErrors.UserNotFound.Description);
            }

            bool hasActiveRefreshToken = user.RefreshTokens.Any(rt => rt.IsActive);

            if (hasActiveRefreshToken)
            {
                user.RefreshTokens.First().Revoke(_dateTime.UtcNow);
            }

            return Result<Unit>.NoContent();
        }
    }
}
