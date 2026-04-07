using FluxStore.Api.Shared.Abstractions;

namespace FluxStore.Api.Domain.UserAggregate.Events
{
    public sealed record ProfileUpdated(
        Guid UserId,
        string DisplayName,
        string? Phone,
        string? AvatarUrl) : IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
    }
}
