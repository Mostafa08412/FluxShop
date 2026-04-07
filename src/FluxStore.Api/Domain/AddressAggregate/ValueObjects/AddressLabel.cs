using Ardalis.Result;
using FluentValidation.Results;
using FluxStore.Api.Domain;

namespace FluxStore.Api.Domain.AddressAggregate.ValueObjects
{
    public sealed class AddressLabel : ValueObject
    {
        public string Value { get; }

        private AddressLabel(string value)
        {
            Value = value;
        }

        public static Result<AddressLabel> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Invalid("Address_AddressLabelRequired__Label", "Label is required.", "Label");
            }

            if (value.Length > 50)
            {
                return Invalid("Address_AddressLabelTooLong__Label", "Label must not exceed 50 characters.", "Label");
            }

            return Result<AddressLabel>.Success(new AddressLabel(value));
        }

        private static Result<AddressLabel> Invalid(string code, string message, string identifier) =>
            Result<AddressLabel>.Invalid(new[]
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
