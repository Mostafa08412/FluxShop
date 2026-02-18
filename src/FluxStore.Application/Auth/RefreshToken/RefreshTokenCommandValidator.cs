using FluentValidation;
using FluxStore.Application.Common.Errors;

namespace FluxStore.Application.Auth.RefreshToken
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(X => X.refreshToken)
                .NotEmpty().WithErrorCode(ApplicationErrors.IdentityErrors.RefreshTokenIsRequired.Code)
                .NotNull().WithErrorCode(ApplicationErrors.IdentityErrors.RefreshTokenIsRequired.Code);
        }
    }

}

