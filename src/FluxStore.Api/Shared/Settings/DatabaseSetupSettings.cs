namespace FluxStore.Api.Shared.Settings
{
    public class DatabaseSetupSettings
    {
        public const string SectionName = "DatabaseSetupSettings";
        public bool ResetOnStartup { get; set; }
        public bool EnsureCreated { get; set; }
        public bool SeedInitialData { get; set; }

    }
}
