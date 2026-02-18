using FluentValidation;
using FluxStore.Application.Common.Errors;

namespace FluxStore.Application.Auth.GoogleLogin
{
    public class GoogleLoginCommandValidator : AbstractValidator<GoogleLoginCommand>
    {
        public GoogleLoginCommandValidator()
        {
            RuleFor(x => x.IdToken)
                .NotEmpty().WithErrorCode(ApplicationErrors.IdentityErrors.GoogleIdTokenIsRequired.Code)
                .NotNull().WithErrorCode(ApplicationErrors.IdentityErrors.GoogleIdTokenIsRequired.Code);
        }
    }
}
