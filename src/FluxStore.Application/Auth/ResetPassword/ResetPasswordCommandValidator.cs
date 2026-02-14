using FluentValidation;

namespace FluxStore.Application.Auth.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.EmailAddress).NotEmpty().NotNull().EmailAddress();
            RuleFor(x => x.ResetPasswordToken).NotEmpty().NotNull();
            RuleFor(x => x.NewPassword).NotEmpty().NotNull();
        }
    }
}
