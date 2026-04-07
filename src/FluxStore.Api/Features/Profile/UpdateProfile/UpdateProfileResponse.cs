namespace FluxStore.Api.Features.Profile.UpdateProfile
{
    public sealed record UpdateProfileResponse(
        Guid UserId,
        string DisplayName,
        string Email,
        string? Phone,
        string? AvatarUrl);
}
