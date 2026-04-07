using Ardalis.Result;
using FluentValidation.Results;
using FluxStore.Api.Domain;

namespace FluxStore.Api.Domain.PaymentMethodAggregate.ValueObjects
{
    public sealed class Last4Digits : ValueObject
    {
        public string Value { get; }

        private Last4Digits(string value)
        {
            Value = value;
        }

        public static Result<Last4Digits> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length != 4 || !value.All(char.IsDigit))
            {
                return Invalid("Payment_InvalidCard__Last4", "Last 4 digits must contain exactly 4 digits.", "Last4");
            }

            return Result<Last4Digits>.Success(new Last4Digits(value));
        }

        private static Result<Last4Digits> Invalid(string code, string message, string identifier) =>
            Result<Last4Digits>.Invalid(new[]
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
