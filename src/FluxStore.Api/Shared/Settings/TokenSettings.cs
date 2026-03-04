namespace FluxStore.Api.Shared.Settings
{
    public sealed class TokenSettings
    {
        public const string SectionName = "TokenSettings";

        public string Issuer { get; init; } = null!;

        public string Audience { get; init; } = null!;

        public string SecretKey { get; init; } = null!;

        public int AccessTokenExpiryInMinutes { get; init; }

        public int RefreshTokenExpiryInMinutes { get; init; }
    }
}
