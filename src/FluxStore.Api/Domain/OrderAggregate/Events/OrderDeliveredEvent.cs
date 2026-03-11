using FluxStore.Api.Shared.Abstractions;

namespace FluxStore.Api.Domain.OrderAggregate.Events
{
    public record OrderDeliveredEvent(
        Guid OrderId,
        DateTime DeliveredAt) : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
