using FluentValidation;
using FluxStore.Application.Common.Errors;
using FluxStore.Domain.Core.Errors;

namespace FluxStore.Application.Auth.VerifyResetPasswordOtp
{
    public class VerifyResetPasswordOtpCommandValidator : AbstractValidator<VerifyResetPasswordOtpCommand>
    {
        public VerifyResetPasswordOtpCommandValidator()
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty().WithErrorCode(Errors.UserErrors.EmailIsRequired.Code)
                .NotNull().WithErrorCode(Errors.UserErrors.EmailIsRequired.Code)
                .EmailAddress().WithErrorCode(Errors.UserErrors.InvalidEmail.Code);
            RuleFor(x => x.Otp)
                .NotEmpty().WithErrorCode(ApplicationErrors.IdentityErrors.OtpIsRequired.Code)
                .NotNull().WithErrorCode(ApplicationErrors.IdentityErrors.OtpIsRequired.Code);
        }
    }
}
