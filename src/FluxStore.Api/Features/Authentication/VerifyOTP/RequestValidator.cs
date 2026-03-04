using FluentValidation;
using FluxStore.Api.Shared.Errors;

namespace FluxStore.Api.Features.Authentication.VerifyOTP
{
    public class RequestValidator : AbstractValidator<VerifyOTPRequest>
    {
        public RequestValidator()
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .WithMessage(UserErrors.EmailAddressIsRequired.Description)
                .WithErrorCode(UserErrors.EmailAddressIsRequired.Code)
                .WithName(nameof(VerifyOTPRequest.EmailAddress))

                .EmailAddress()
                .WithMessage(UserErrors.InvalidEmailAddress.Description)
                .WithErrorCode(UserErrors.InvalidEmailAddress.Code)
                .WithName(nameof(VerifyOTPRequest.EmailAddress));

            RuleFor(x => x.Otp)
                .NotEmpty()
                .Length(6);
        }
    }
}
