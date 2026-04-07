using FluxStore.Api.Shared.Abstractions;

namespace FluxStore.Api.Domain.PaymentMethodAggregate.Events
{
    public sealed record DefaultPaymentMethodChanged(
        Guid PaymentMethodId,
        Guid UserId) : IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
    }
}
