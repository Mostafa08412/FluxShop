using Ardalis.Result;
using FluentValidation.Results;
using FluxStore.Api.Domain;

namespace FluxStore.Api.Domain.AddressAggregate.ValueObjects
{
    public sealed class Country : ValueObject
    {
        public string Name { get; }

        private Country(string name)
        {
            Name = name;
        }

        public static Result<Country> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Invalid("Address_CountryRequired__Country", "Country is required.", "Country");
            }

            if (name.Length > 100)
            {
                return Invalid("Address_CountryTooLong__Country", "Country must not exceed 100 characters.", "Country");
            }

            return Result<Country>.Success(new Country(name));
        }

        private static Result<Country> Invalid(string code, string message, string identifier) =>
            Result<Country>.Invalid(new[]
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
            yield return Name;
        }
    }
}
