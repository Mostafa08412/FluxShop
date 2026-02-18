using FluentValidation;
using FluxStore.Application.Common.Errors;

namespace FluxStore.Application.Auth.ChangePassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {

            RuleFor(X => X.CurrentPassword)
                .NotEmpty().WithErrorCode(ApplicationErrors.IdentityErrors.CurrentPasswordIsRequired.Code)
                .NotNull().WithErrorCode(ApplicationErrors.IdentityErrors.CurrentPasswordIsRequired.Code);
            RuleFor(X => X.NewPassword)
                .NotEmpty().WithErrorCode(ApplicationErrors.IdentityErrors.NewPasswordIsRequired.Code)
                .NotNull().WithErrorCode(ApplicationErrors.IdentityErrors.NewPasswordIsRequired.Code);
            RuleFor(X => X.ConfirmNewPassword)
                .NotEmpty().WithErrorCode(ApplicationErrors.IdentityErrors.ConfirmNewPasswordIsRequired.Code)
                .NotNull().WithErrorCode(ApplicationErrors.IdentityErrors.ConfirmNewPasswordIsRequired.Code)
                .Equal(X => X.NewPassword).WithErrorCode(ApplicationErrors.IdentityErrors.ConfirmPasswordMismatch.Code);
        }
    }
}
