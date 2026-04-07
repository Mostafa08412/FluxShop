using Ardalis.Result;
using FluxStore.Api.Domain.PaymentMethodAggregate.ValueObjects;

namespace FluxStore.Api.Domain
{
    public interface IPaymentGateway
    {
        Task<Result<GatewayToken>> TokenizeCard(
            string cardNumber,
            int expiryMonth,
            int expiryYear,
            string cvv,
            CancellationToken ct);
    }
}
