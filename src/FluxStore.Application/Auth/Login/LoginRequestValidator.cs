using FluentValidation;

namespace FluxStore.Application.Auth.Login
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {

            RuleFor(X => X.EmailAddress).NotEmpty().NotNull();

            RuleFor(X => X.Password).NotEmpty().NotNull();
        }
    }

}

