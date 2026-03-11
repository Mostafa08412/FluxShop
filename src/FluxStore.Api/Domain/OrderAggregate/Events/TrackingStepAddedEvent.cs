using FluxStore.Api.Domain.Enums;
using FluxStore.Api.Shared.Abstractions;

namespace FluxStore.Api.Domain.OrderAggregate.Events
{
    public record TrackingStepAddedEvent(
        Guid OrderId,
        TrackingStatus TrackingStatus,
        string Description) : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
