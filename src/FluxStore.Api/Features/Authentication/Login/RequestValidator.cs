using FluentValidation;
using FluxStore.Api.Shared.Errors;

namespace FluxStore.Api.Features.Authentication.Login
{
    public class RequestValidator : AbstractValidator<LoginRequest>
    {
        public RequestValidator()
        {
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
                .WithName(nameof(LoginRequest.EmailAddress));

            RuleFor(x => x.Password)
                .NotEmpty()
                    .WithMessage(IdentityErrors.PasswordIsRequired.Description)
                    .WithErrorCode(IdentityErrors.PasswordIsRequired.Code)
                .WithName(nameof(LoginRequest.Password));
        }
    }
}
