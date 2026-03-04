using Ardalis.Result;
using FluxStore.Api.Infrastructure.Persistence;
using FluxStore.Api.Infrastructure.Services;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FluxStore.Api.Features.Authentication.ChangePassword
{

    public class RequestHandler : IRequestHandler<ChangePasswordRequest, Result<Unit>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CurrentUser _currentUser;

        public RequestHandler(UserManager<ApplicationUser> userManager, CurrentUser currentUser)
        {
            _userManager = userManager;
            _currentUser = currentUser;
        }

        public async Task<Result<Unit>> Handle(ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(_currentUser.UserId.ToString());

            if (user is null)
                return Result<Unit>.Unauthorized(IdentityErrors.InvalidCredentials.Code, IdentityErrors.InvalidCredentials.Description);

            var changeResult = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!changeResult.Succeeded)
                return changeResult.ToResult();

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
