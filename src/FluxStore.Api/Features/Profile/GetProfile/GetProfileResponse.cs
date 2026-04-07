namespace FluxStore.Api.Features.Profile.GetProfile
{
    public sealed record GetProfileResponse(
        Guid UserId,
        string DisplayName,
        string Email,
        string? Phone,
        string? AvatarUrl);
}
