using FluentValidation;
using FluxStore.Api.Shared.Errors;

namespace FluxStore.Api.Features.Authentication.ResetPassword
{
    public class RequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public RequestValidator()
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .WithErrorCode(UserErrors.EmailAddressIsRequired.Code)
                .WithMessage(UserErrors.EmailAddressIsRequired.Description)
                .WithName(x => nameof(ResetPasswordRequest.EmailAddress))
                .EmailAddress()
                .WithErrorCode(UserErrors.InvalidEmailAddress.Code)
                .WithMessage(UserErrors.InvalidEmailAddress.Description)
                .WithName(x => nameof(ResetPasswordRequest.EmailAddress));

            RuleFor(x => x.ResetPasswordToken)
                .NotEmpty()
                .WithErrorCode(IdentityErrors.ResetPasswordTokenIsRequired.Code)
                .WithMessage(IdentityErrors.ResetPasswordTokenIsRequired.Description)
                .WithName(x => nameof(ResetPasswordRequest.ResetPasswordToken));

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithErrorCode(IdentityErrors.PasswordIsRequired.Code)
                .WithMessage(IdentityErrors.PasswordIsRequired.Description)
                .WithName(x => nameof(ResetPasswordRequest.NewPassword));


        }
    }
}
