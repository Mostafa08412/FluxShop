using FluxStore.Api.Shared.Abstractions;

namespace FluxStore.Api.Domain.PaymentMethodAggregate.Events
{
    public sealed record PaymentMethodRemoved(Guid PaymentMethodId) : IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
    }
}
