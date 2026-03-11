using FluxStore.Api.Shared.Abstractions;

namespace FluxStore.Api.Domain.OrderAggregate.Events
{
    public record OrderConfirmedEvent(
        Guid OrderId,
        Guid? PaymentId) : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
