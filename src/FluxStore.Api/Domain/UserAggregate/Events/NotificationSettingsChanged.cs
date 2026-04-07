using FluxStore.Api.Domain.UserAggregate.ValueObjects;
using FluxStore.Api.Shared.Abstractions;

namespace FluxStore.Api.Domain.UserAggregate.Events
{
    public sealed record NotificationSettingsChanged(
        Guid UserId,
        NotificationSettings Settings) : IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
    }
}
