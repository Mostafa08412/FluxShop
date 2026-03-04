using FluentValidation;
using FluxStore.Api.Shared.Errors;

namespace FluxStore.Api.Features.Authentication.Refresh
{
    public class RequestValidator : AbstractValidator<Endpoint>
    {
        public RequestValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithErrorCode(IdentityErrors.RefreshTokenIsRequired.Code)
                .WithMessage(IdentityErrors.RefreshTokenIsRequired.Description)
                .WithName(nameof(Endpoint.RefreshToken));
        }
    }

}
