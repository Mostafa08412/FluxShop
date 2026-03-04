using FluentValidation;
using FluxStore.Api.Shared.Errors;

namespace FluxStore.Api.Features.Authentication.GoogleLogin
{
    public class RequestValidator : AbstractValidator<GoogleLoginRequest>
    {
        public RequestValidator()
        {
            RuleFor(x => x.IdToken)
                .NotEmpty()
                .WithMessage(IdentityErrors.GoogleIdTokenIsRequired.Code)
                .WithErrorCode(IdentityErrors.GoogleIdTokenIsRequired.Code)
                .WithName(nameof(GoogleLoginRequest.IdToken));
        }
    }
}
