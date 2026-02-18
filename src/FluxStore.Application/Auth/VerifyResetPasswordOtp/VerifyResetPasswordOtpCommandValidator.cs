using FluentValidation;
using FluxStore.Application.Common.Errors;

namespace FluxStore.Application.Auth.VerifyResetPasswordOtp
{
    public class VerifyResetPasswordOtpCommandValidator : AbstractValidator<VerifyResetPasswordOtpCommand>
    {
        public VerifyResetPasswordOtpCommandValidator()
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty().WithErrorCode(ApplicationErrors.IdentityErrors.EmailIsRequired.Code)
                .NotNull().WithErrorCode(ApplicationErrors.IdentityErrors.EmailIsRequired.Code)
                .EmailAddress().WithErrorCode(ApplicationErrors.IdentityErrors.InvalidEmail.Code);
            RuleFor(x => x.Otp)
                .NotEmpty().WithErrorCode(ApplicationErrors.IdentityErrors.OtpIsRequired.Code)
                .NotNull().WithErrorCode(ApplicationErrors.IdentityErrors.OtpIsRequired.Code);
        }
    }
}
