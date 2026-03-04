namespace FluxStore.Api.Shared.Settings
{
    public class ExternalAuthenticationSettings
    {
        public const string SectionName = "ExternalAuthenticationSettings";
        public GoogleAuthenticationSettings Google { get; set; }
        public GoogleAuthenticationSettings Apple { get; set; }

    }


    public class GoogleAuthenticationSettings
    {
        public string ClientId { get; set; }
    }

    public class AppleAuthenticationSettings
    {
        public string ClientId { get; set; }


    }

}
