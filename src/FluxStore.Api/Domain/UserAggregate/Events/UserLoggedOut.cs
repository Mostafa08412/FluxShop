using FluxStore.Api.Shared.Abstractions;

namespace FluxStore.Api.Domain.UserAggregate.Events
{
    public sealed record UserLoggedOut(
        Guid UserId,
        string? DeviceId) : IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
    }
}
