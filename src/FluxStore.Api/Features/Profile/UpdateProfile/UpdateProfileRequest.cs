using Ardalis.Result;
using FluxStore.Api.Markers;
using MediatR;

namespace FluxStore.Api.Features.Profile.UpdateProfile
{
    public sealed record UpdateProfileRequest : IRequest<Result<UpdateProfileResponse>>, ICommand
    {
        public string DisplayName { get; init; } = string.Empty;
        public string? Phone { get; init; }
        public string? AvatarUrl { get; init; }
    }
}
