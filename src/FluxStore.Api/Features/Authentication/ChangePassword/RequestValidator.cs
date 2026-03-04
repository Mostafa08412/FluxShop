using FluentValidation;
using FluxStore.Api.Shared.Errors;

namespace FluxStore.Api.Features.Authentication.ChangePassword
{
    public class RequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public RequestValidator()
        {

            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .WithMessage(IdentityErrors.CurrentPasswordIsRequired.Description)
                .WithErrorCode(IdentityErrors.PasswordIsRequired.Code)
                .WithName(nameof(ChangePasswordRequest.CurrentPassword));

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage(IdentityErrors.NewPasswordIsRequired.Description)
                .WithErrorCode(IdentityErrors.NewPasswordIsRequired.Code)
                .WithName(nameof(ChangePasswordRequest.NewPassword));
        }
    }
}
