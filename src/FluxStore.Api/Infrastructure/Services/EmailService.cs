using Ardalis.Result;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluxStore.Api.Shared.Abstractions;
using FluxStore.Api.Shared.Settings;
using MediatR;
using Microsoft.Extensions.Options;

namespace FluxStore.Api.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly string TemplatesDirectory = Path.Combine(AppContext.BaseDirectory, "Shared", "EmailRazorTemplates");

        private readonly ILogger<EmailService> _logger;

        private readonly SmtpSettings _smtpSettings;

        private readonly IFluentEmail _fluentEmail;


        public EmailService(ILogger<EmailService> logger, IOptions<SmtpSettings> smtpSettings, IFluentEmail fluentEmail)
        {
            _logger = logger;
            _smtpSettings = smtpSettings.Value;
            _fluentEmail = fluentEmail;
        }

        public async Task<Result<Unit>> SendEmail<T>(string to, T emailModel, CancellationToken cancellationToken) where T : IEmailModel
        {
            string templatePath = Path.Combine(TemplatesDirectory, emailModel.TemplateName + ".cshtml");
            try
            {
                _logger.LogInformation("Attempting to send email to {Email} using template {Template}", to, emailModel.TemplateName);

                var sendResult = await _fluentEmail
                        .To(to)
                        .Subject(emailModel.Subject)
                        .UsingTemplateFromFile(templatePath, emailModel)
                        .SendAsync(cancellationToken);

                if (!sendResult.Successful)
                {
                    _logger.LogError("Failed to send email to {Email} using template {Template}", to, emailModel.TemplateName);

                    return Result<Unit>.Unavailable(string.Join(',', sendResult.ErrorMessages));
                }

                _logger.LogInformation("Email successfully send to {emailAddress} using template {model}", to, emailModel.TemplateName);


                return Result<Unit>.NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to send email to {Email} using template {Template} Error {error}", to, emailModel.TemplateName, ex.Message);

                return Result<Unit>.Unavailable(ex.Message);
            }
        }

        public async Task<Result<Unit>> SendBulkEmails<T>(string[] to, T emailModel, CancellationToken cancellationToken) where T : IEmailModel
        {
            string templatePath = Path.Combine(TemplatesDirectory, emailModel.TemplateName + ".cshtml");

            string errorMessage = string.Empty;

            try
            {
                _logger.LogInformation("Attempting to send bulk email to {EmailCount} recipients using template {Template}", to.Length, emailModel.TemplateName);

                var sendResult = await _fluentEmail
                     .To(_smtpSettings.User)
                     .BCC(to.Select(X => new Address(X)))
                     .Subject(emailModel.Subject)
                     .UsingTemplateFromFile(templatePath, emailModel)
                     .SendAsync(cancellationToken);

                if (!sendResult.Successful)
                {
                    throw new Exception(string.Join(',', sendResult.ErrorMessages));

                }

                return Result<Unit>.NoContent();

            }
            catch (Exception ex)
            {

                _logger.LogError("Failed to send bulk email to {EmailCount} recipients using template {Template} Error {error}", to.Length, emailModel.TemplateName, ex.Message);

                throw;

            }
        }


    }
}
