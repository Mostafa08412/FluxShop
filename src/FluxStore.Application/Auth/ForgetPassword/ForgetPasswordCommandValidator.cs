using FluentValidation;

namespace FluxStore.Application.Auth.ForgetPassword
{
    public class ForgetPasswordCommandValidator : AbstractValidator<ForgetPasswordCommand>
    {
        public ForgetPasswordCommandValidator()
        {
            RuleFor(x => x.EmailAddress).NotEmpty().NotNull().EmailAddress();
        }
    }
}
