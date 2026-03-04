namespace FluxStore.Api.Shared.Settings
{
    public class SmtpSettings
    {
        public static string SectionName => "SmtpSettings";
        public string Name { get; set; } = "FluxStore";
        public string Server { get; set; } = string.Empty;
        public int Port { get; set; } = 25;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool UseSsl { get; set; } = false;
        public bool RequiresAuthentication { get; set; } = false;

    }
}
