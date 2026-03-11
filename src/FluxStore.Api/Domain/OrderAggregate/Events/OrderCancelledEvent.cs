using FluxStore.Api.Shared.Abstractions;

namespace FluxStore.Api.Domain.OrderAggregate.Events
{
    public record OrderCancelledEvent(
        Guid OrderId,
        string Reason,
        bool isPaid,
        DateTime CancelledAt) : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
