using FluxStore.Api.Domain.PaymentMethodAggregate.ValueObjects;
using FluxStore.Api.Shared.Abstractions;

namespace FluxStore.Api.Domain.PaymentMethodAggregate.Events
{
    public sealed record PaymentMethodAdded(
        Guid PaymentMethodId,
        Guid UserId,
        Last4Digits Last4,
        CardBrand Brand,
        int ExpiryMonth,
        int ExpiryYear,
        bool IsDefault) : IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
    }
}
