namespace FluxStore.Api.Domain
{
    public interface ITokenBlacklist
    {
        Task BlacklistToken(string jti, TimeSpan ttl, CancellationToken ct);

        Task<bool> IsBlacklisted(string jti, CancellationToken ct);
    }
}
