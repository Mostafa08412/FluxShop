using FluentValidation;
using FluxStore.Application.Common.Errors;

namespace FluxStore.Application.Auth.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty().WithErrorCode(ApplicationErrors.IdentityErrors.EmailIsRequired.Code)
                .NotNull().WithErrorCode(ApplicationErrors.IdentityErrors.EmailIsRequired.Code)
                .EmailAddress().WithErrorCode(ApplicationErrors.IdentityErrors.InvalidEmail.Code);
            RuleFor(x => x.Name)
                .NotNull().WithErrorCode(ApplicationErrors.IdentityErrors.NameIsRequired.Code)
                .NotEmpty().WithErrorCode(ApplicationErrors.IdentityErrors.NameIsRequired.Code)
                .Must(X => X.Trim().Split(' ').Length == 2)
                    .WithErrorCode(ApplicationErrors.IdentityErrors.InvalidNameFormat.Code)
                    .When(X => !string.IsNullOrWhiteSpace(X.Name));
            RuleFor(x => x.Password)
                .NotNull().WithErrorCode(ApplicationErrors.IdentityErrors.PasswordIsRequired.Code)
                .NotEmpty().WithErrorCode(ApplicationErrors.IdentityErrors.PasswordIsRequired.Code);
        }
    }
}
