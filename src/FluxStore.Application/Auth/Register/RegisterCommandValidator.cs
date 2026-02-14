using FluentValidation;

namespace FluxStore.Application.Auth.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.EmailAddress)
                .EmailAddress()
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.Name)
                .Must(X => X.Trim().Split(' ').Length == 2)
                    .WithMessage("Name must consist of first name and last name separated by a space.")
                    .When(X => string.IsNullOrWhiteSpace(X.Name) == false)
                .NotNull()
                .NotEmpty();
            RuleFor(x => x.Password)
                .NotNull()
                .NotEmpty();
        }
    }
}
