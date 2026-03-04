using FluentValidation;
using FluxStore.Api.Shared.Errors;

namespace FluxStore.Api.Features.Authentication.ForgetPassword
{
    public class RequestValidator : AbstractValidator<ForgetPasswordRequest>
    {
        public RequestValidator()
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .WithErrorCode(UserErrors.EmailAddressIsRequired.Code)
                .WithMessage(UserErrors.EmailAddressIsRequired.Description)
                .WithName(x => nameof(ForgetPasswordRequest.EmailAddress))

                .EmailAddress()
                .WithErrorCode(UserErrors.InvalidEmailAddress.Code)
                .WithMessage(UserErrors.InvalidEmailAddress.Description)
                .WithName(x => nameof(ForgetPasswordRequest.EmailAddress));
        }
    }












}
