using FluentValidation;
using FluxStore.Application.Common.Errors;
using FluxStore.Domain.Core.Errors;

namespace FluxStore.Application.Auth.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty().WithErrorCode(Errors.UserErrors.EmailIsRequired.Code)
                .NotNull().WithErrorCode(Errors.UserErrors.EmailIsRequired.Code)
                .EmailAddress().WithErrorCode(Errors.UserErrors.InvalidEmail.Code);
            RuleFor(x => x.Name)
                 .NotEmpty()
                     .WithErrorCode(Errors.UserErrors.NameIsRequired.Code)

                 .Must(name =>
                 {
                     if (string.IsNullOrWhiteSpace(name)) return false;
                     return name.Trim().Split(' ').Length == 2;
                 })
                 .WithErrorCode(Errors.UserErrors.InvalidNameFormat.Code);
            RuleFor(x => x.Password)
                .NotNull().WithErrorCode(ApplicationErrors.IdentityErrors.PasswordIsRequired.Code)
                .NotEmpty().WithErrorCode(ApplicationErrors.IdentityErrors.PasswordIsRequired.Code);
        }
    }
}
