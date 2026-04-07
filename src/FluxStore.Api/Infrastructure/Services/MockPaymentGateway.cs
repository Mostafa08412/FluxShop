using System.Collections.Concurrent;
using Ardalis.Result;
using FluxStore.Api.Domain;
using FluxStore.Api.Domain.PaymentMethodAggregate.ValueObjects;
using FluxStore.Api.Shared.Errors;

namespace FluxStore.Api.Infrastructure.Services
{
    public sealed class MockPaymentGateway : IPaymentGateway
    {
        private readonly ConcurrentDictionary<string, int> _attempts = new();

        public Task<Result<GatewayToken>> TokenizeCard(
            string cardNumber,
            int expiryMonth,
            int expiryYear,
            string cvv,
            CancellationToken ct)
        {
            var attempt = _attempts.AddOrUpdate(cardNumber, 1, (_, current) => current + 1);

            if (cardNumber.EndsWith("0000", StringComparison.Ordinal) && attempt <= 2)
            {
                return Task.FromResult(Result<GatewayToken>.Error(new ErrorList([PaymentErrors.TokenizationFailed.Code, PaymentErrors.TokenizationFailed.Description])));
            }

            var last4 = cardNumber.Length >= 4 ? cardNumber[^4..] : cardNumber;
            var tokenValue = $"tok_test_{last4}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

            return Task.FromResult(GatewayToken.Create(tokenValue));
        }
    }
}