using FluentValidation;
using FluxStore.Application.Common.Errors;
using FluxStore.Domain.Core.Errors;

namespace FluxStore.Application.Auth.Login
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {

            RuleFor(X => X.EmailAddress)
                .NotEmpty().WithErrorCode(Errors.UserErrors.EmailIsRequired.Code)
                .NotNull().WithErrorCode(Errors.UserErrors.EmailIsRequired.Code);

            RuleFor(X => X.Password)
                .NotEmpty().WithErrorCode(ApplicationErrors.IdentityErrors.PasswordIsRequired.Code)
                .NotNull().WithErrorCode(ApplicationErrors.IdentityErrors.PasswordIsRequired.Code);
        }
    }

}


