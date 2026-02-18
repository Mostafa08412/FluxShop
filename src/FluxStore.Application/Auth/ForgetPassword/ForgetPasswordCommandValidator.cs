using FluentValidation;
using FluxStore.Application.Common.Errors;

namespace FluxStore.Application.Auth.ForgetPassword
{
    public class ForgetPasswordCommandValidator : AbstractValidator<ForgetPasswordCommand>
    {
        public ForgetPasswordCommandValidator()
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty().WithErrorCode(ApplicationErrors.IdentityErrors.EmailIsRequired.Code)
                .NotNull().WithErrorCode(ApplicationErrors.IdentityErrors.EmailIsRequired.Code)
                .EmailAddress().WithErrorCode(ApplicationErrors.IdentityErrors.InvalidEmail.Code);
        }
    }
}
