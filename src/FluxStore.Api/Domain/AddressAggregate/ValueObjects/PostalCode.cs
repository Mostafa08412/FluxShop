using Ardalis.Result;
using FluentValidation.Results;
using FluxStore.Api.Domain;

namespace FluxStore.Api.Domain.AddressAggregate.ValueObjects
{
    public sealed class PostalCode : ValueObject
    {
        public string Value { get; }

        private PostalCode(string value)
        {
            Value = value;
        }

        public static Result<PostalCode> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Invalid("Address_PostalCodeRequired__PostalCode", "Postal code is required.", "PostalCode");
            }

            if (value.Length > 20)
            {
                return Invalid("Address_PostalCodeTooLong__PostalCode", "Postal code must not exceed 20 characters.", "PostalCode");
            }

            return Result<PostalCode>.Success(new PostalCode(value));
        }

        private static Result<PostalCode> Invalid(string code, string message, string identifier) =>
            Result<PostalCode>.Invalid(new[]
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
