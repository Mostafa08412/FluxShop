using FluentValidation;
using FluxStore.Application.Common.Errors;

namespace FluxStore.Application.Auth.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty().WithErrorCode(ApplicationErrors.IdentityErrors.EmailIsRequired.Code)
                .NotNull().WithErrorCode(ApplicationErrors.IdentityErrors.EmailIsRequired.Code)
                .EmailAddress().WithErrorCode(ApplicationErrors.IdentityErrors.InvalidEmail.Code);
            RuleFor(x => x.ResetPasswordToken)
                .NotEmpty().WithErrorCode(ApplicationErrors.IdentityErrors.ResetPasswordTokenIsRequired.Code)
                .NotNull().WithErrorCode(ApplicationErrors.IdentityErrors.ResetPasswordTokenIsRequired.Code);
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithErrorCode(ApplicationErrors.IdentityErrors.NewPasswordIsRequired.Code)
                .NotNull().WithErrorCode(ApplicationErrors.IdentityErrors.NewPasswordIsRequired.Code);
        }
    }
}
