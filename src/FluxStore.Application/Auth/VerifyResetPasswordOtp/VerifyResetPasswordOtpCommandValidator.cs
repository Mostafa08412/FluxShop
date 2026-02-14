using FluentValidation;

namespace FluxStore.Application.Auth.VerifyResetPasswordOtp
{
    public class VerifyResetPasswordOtpCommandValidator : AbstractValidator<VerifyResetPasswordOtpCommand>
    {
        public VerifyResetPasswordOtpCommandValidator()
        {
            RuleFor(x => x.EmailAddress).NotEmpty().NotNull().EmailAddress();
            RuleFor(x => x.Otp).NotEmpty().NotNull();
        }
    }
}
