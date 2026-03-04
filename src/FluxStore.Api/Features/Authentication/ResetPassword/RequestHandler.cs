using Ardalis.Result;
using FluxStore.Api.Infrastructure.Persistence;
using FluxStore.Api.Markers;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FluxStore.Api.Features.Authentication.ResetPassword
{
    public record ResetPasswordRequest(string EmailAddress, string ResetPasswordToken, string NewPassword) : IRequest<Result<Unit>>, ICommand;

    public class RequestHandler : IRequestHandler<ResetPasswordRequest, Result<Unit>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RequestHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<Unit>> Handle(ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.EmailAddress);

            if (user is null)
            {
                return Result<Unit>.NotFound(IdentityErrors.UserNotFoundByEmail.Code, IdentityErrors.UserNotFoundByEmail.Description);
            }

            var resetResult = await _userManager.ResetPasswordAsync(user, request.ResetPasswordToken, request.NewPassword);

            if (!resetResult.Succeeded)
            {
                return resetResult.ToResult();
            }

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
