using FluentValidation;
using FluxStore.Application.Common.Errors;

namespace FluxStore.Application.Auth.Login
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {

            RuleFor(X => X.EmailAddress)
                .NotEmpty().WithErrorCode(ApplicationErrors.IdentityErrors.EmailIsRequired.Code)
                .NotNull().WithErrorCode(ApplicationErrors.IdentityErrors.EmailIsRequired.Code);

            RuleFor(X => X.Password)
                .NotEmpty().WithErrorCode(ApplicationErrors.IdentityErrors.PasswordIsRequired.Code)
                .NotNull().WithErrorCode(ApplicationErrors.IdentityErrors.PasswordIsRequired.Code);
        }
    }

}


