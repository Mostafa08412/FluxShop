using FluxStore.Api.Shared.Abstractions;

namespace FluxStore.Api.Domain.OrderAggregate.Events
{
    public record OrderPlacedEvent(
        Guid OrderId,
        Guid UserId,
        decimal Total,
        string PaymentMethod) : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
