using FluxStore.Api.Domain.PaymentMethodAggregate;

namespace FluxStore.Api.Features.Profile.AddPaymentMethod
{
    public sealed record AddPaymentMethodResponse(
        Guid Id,
        string Last4,
        CardBrand Brand,
        int ExpiryMonth,
        int ExpiryYear,
        bool IsDefault);
}