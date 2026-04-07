using Ardalis.Result;
using FluentValidation.Results;
using FluxStore.Api.Domain;

namespace FluxStore.Api.Domain.AddressAggregate.ValueObjects
{
    public sealed class Street : ValueObject
    {
        public string Value { get; }

        private Street(string value)
        {
            Value = value;
        }

        public static Result<Street> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Invalid("Address_StreetRequired__Street", "Street is required.", "Street");
            }

            if (value.Length > 200)
            {
                return Invalid("Address_StreetTooLong__Street", "Street must not exceed 200 characters.", "Street");
            }

            return Result<Street>.Success(new Street(value));
        }

        private static Result<Street> Invalid(string code, string message, string identifier) =>
            Result<Street>.Invalid(new[]
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
