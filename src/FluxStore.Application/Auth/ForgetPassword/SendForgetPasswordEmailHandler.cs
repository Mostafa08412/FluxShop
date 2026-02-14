using FluxStore.Application.Common.Interfaces;
using MediatR;

namespace FluxStore.Application.Auth.ForgetPassword
{
    public class SendForgetPasswordEmailHandler : IRequestHandler<SendForgetPasswordEmail>
    {
        private readonly IEmailService _emailService;

        public SendForgetPasswordEmailHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task Handle(SendForgetPasswordEmail request, CancellationToken cancellationToken)
        {
            await _emailService.SendForgetPasswordEmail(
                request.To,
                request.Name,
                request.EmailAddress,
                request.Otp);
        }
    }
}
