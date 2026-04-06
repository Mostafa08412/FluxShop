using FluxStore.Api.Shared.Abstractions;

namespace FluxStore.Api.Domain.ProductAggregate.Events;

public record ProductCreatedEvent(
    Guid ProductId,
    string ProductName,
    Guid CategoryId,
    decimal Price) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
