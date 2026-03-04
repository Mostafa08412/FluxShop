using FluentValidation;
using FluxStore.Api.Shared.Errors;

namespace FluxStore.Api.Features.Authentication.Register
{
    public class RequestValidator : AbstractValidator<RegisterRequest>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithErrorCode(UserErrors.NameIsRequired.Code)
                .Must(name =>
                    {
                        if (string.IsNullOrWhiteSpace(name)) return false;
                        return name.Trim().Split(' ').Length == 2;
                    })
                .WithErrorCode(UserErrors.InvalidNameFormat.Code);


            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                    .WithMessage(UserErrors.EmailAddressIsRequired.Description)
                    .WithErrorCode(UserErrors.EmailAddressIsRequired.Code)
                .EmailAddress()
                    .WithMessage(UserErrors.InvalidEmailAddress.Description)
                    .WithErrorCode(UserErrors.InvalidEmailAddress.Code)
                .MaximumLength(256)
                    .WithMessage(UserErrors.EmailAddressTooLong.Description)
                    .WithErrorCode(UserErrors.EmailAddressTooLong.Code)
                .WithName(nameof(RegisterRequest.EmailAddress));

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(IdentityErrors.PasswordIsRequired.Description)
                .WithErrorCode(IdentityErrors.PasswordIsRequired.Code)
                .WithName(nameof(RegisterRequest.Password));

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage(IdentityErrors.ConfirmNewPasswordIsRequired.Description)
                .WithErrorCode(IdentityErrors.ConfirmNewPasswordIsRequired.Code)
                .WithName(nameof(RegisterRequest.Password))

                .Equal(x => x.Password)
                .WithMessage(IdentityErrors.ConfirmPasswordMismatch.Description)
                .WithErrorCode(IdentityErrors.ConfirmPasswordMismatch.Code)
                .WithName(nameof(RegisterRequest.ConfirmPassword));
        }
    }
}
