using FluentEmail.Core;
using FluxStore.Application.Common.Interfaces;
using FluxStore.Infrastructure.EmailServices.EmailTemplates;
using FluxStore.Infrastructure.EmailServices.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;

namespace FluxStore.Infrastructure.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly IFluentEmail fluentEmail;
        private readonly SmtpSettings smtpSettings;
        private readonly ILogger<EmailService> logger;

        public EmailService(IFluentEmail fluentEmail, IOptions<SmtpSettings> smtpSettings, ILogger<EmailService> logger)
        {
            this.fluentEmail = fluentEmail;
            this.smtpSettings = smtpSettings.Value;
            this.logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await fluentEmail.To(to).Body(body).Subject(subject).SendAsync();
        }

        public async Task SendForgetPasswordEmail(string to, string name, string emailAddress, string otp)
        {

            ForgetPasswordEmailModel model = new ForgetPasswordEmailModel
            {
                Name = name,
                EmailAddress = emailAddress,
                Otp = otp
            };

            string subject = "Forget Password Request";

            string templatePath = Path.Combine(AppContext.BaseDirectory, "EmailServices", "EmailTemplates", "ForgetPasswordEmailTemplate.cshtml");

            ServicePointManager.FindServicePoint(new Uri($"http://{smtpSettings.SmtpPort}")).ConnectionLimit = 1;

            await fluentEmail
                .To(to)
                .Subject(subject)
                .UsingTemplateFromFile(templatePath, model)
                .SendAsync();



        }


    }
}
