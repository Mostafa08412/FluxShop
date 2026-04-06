using FluxStore.Api.Shared.Abstractions;

namespace FluxStore.Api.Domain.ProductAggregate.Events;

public record ProductPublishedEvent(Guid ProductId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
