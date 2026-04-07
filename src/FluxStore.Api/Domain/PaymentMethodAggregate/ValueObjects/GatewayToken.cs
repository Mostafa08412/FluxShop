using Ardalis.Result;
using FluentValidation.Results;
using FluxStore.Api.Domain;

namespace FluxStore.Api.Domain.PaymentMethodAggregate.ValueObjects
{
    public sealed class GatewayToken : ValueObject
    {
        public string Value { get; }

        private GatewayToken(string value)
        {
            Value = value;
        }

        public static Result<GatewayToken> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Invalid("Payment_TokenizationFailed__GatewayToken", "Gateway token is required.", "GatewayToken");
            }

            return Result<GatewayToken>.Success(new GatewayToken(value));
        }

        private static Result<GatewayToken> Invalid(string code, string message, string identifier) =>
            Result<GatewayToken>.Invalid(new[]
            {
                new ValidationError
                {
                    ErrorCode = code,
                    ErrorMessage = message,
                    Identifier = identifier
                }
            });

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}
