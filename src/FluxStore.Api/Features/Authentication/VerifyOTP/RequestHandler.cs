using Ardalis.Result;
using FluxStore.Api.Infrastructure.Persistence;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FluxStore.Api.Features.Authentication.VerifyOTP
{


    public class RequestHandler : IRequestHandler<VerifyOTPRequest, Result<VerifyOtpResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RequestHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<VerifyOtpResponse>> Handle(VerifyOTPRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.EmailAddress);

            if (user is null)
            {
                return Result<VerifyOtpResponse>.NotFound(IdentityErrors.UserNotFound.Code, IdentityErrors.UserNotFound.Description);
            }

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "ResetPasswordOTPProvider", request.Otp);

            if (!isValid)
            {
                return Result<VerifyOtpResponse>.Invalid(IdentityErrors.InvalidOtp.ToValidationError());
            }

            await _userManager.UpdateSecurityStampAsync(user);

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var response = new VerifyOtpResponse(resetToken);

            return Result<VerifyOtpResponse>.Success(response);
        }
    }
}
