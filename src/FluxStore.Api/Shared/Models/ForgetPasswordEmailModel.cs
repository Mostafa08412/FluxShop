using FluxStore.Api.Shared.Abstractions;

namespace FluxStore.Api.Shared.Models
{
    public class ForgetPasswordEmailModel : IEmailModel
    {
        public string TemplateName { get; } = "ForgetPasswordEmailTemplate";
        public string Subject { get; } = "Reset Your Password";
        public string Name { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;

    }
}
