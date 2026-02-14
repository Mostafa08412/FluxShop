namespace FluxStore.Application.Common.Interfaces
{
    public interface IEmailService
    {

        public Task SendEmailAsync(string to, string subject, string body);
        public Task SendForgetPasswordEmail(string to, string name, string emailAddress, string otp);


    }
}
