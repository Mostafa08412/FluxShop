using FluxStore.Api.Domain.PaymentMethodAggregate;

namespace FluxStore.Api.Features.Profile.ListPaymentMethods
{
    public sealed record PaymentMethodDto(
        Guid Id,
        string Last4,
        CardBrand Brand,
        int ExpiryMonth,
        int ExpiryYear,
        bool IsDefault);

    public sealed record ListPaymentMethodsResponse(List<PaymentMethodDto> PaymentMethods);
}