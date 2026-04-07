using FluxStore.Api.Domain.PaymentMethodAggregate;

namespace FluxStore.Api.Features.Profile.SetDefaultPaymentMethod
{
    public sealed record SetDefaultPaymentMethodResponse(
        Guid Id,
        string Last4,
        CardBrand Brand,
        int ExpiryMonth,
        int ExpiryYear,
        bool IsDefault);
}