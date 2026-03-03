using FluentValidation;
using FluxStore.Domain.Core.Errors;

namespace FluxStore.Application.Auth.ForgetPassword
{
    public class ForgetPasswordCommandValidator : AbstractValidator<ForgetPasswordCommand>
    {
        public ForgetPasswordCommandValidator()
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty().WithErrorCode(Errors.UserErrors.EmailIsRequired.Code)
                .NotNull().WithErrorCode(Errors.UserErrors.EmailIsRequired.Code)
                .EmailAddress().WithErrorCode(Errors.UserErrors.InvalidEmail.Code);
        }
    }
}
