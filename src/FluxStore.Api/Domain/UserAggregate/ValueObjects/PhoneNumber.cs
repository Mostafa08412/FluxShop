using Ardalis.Result;
using FluentValidation.Results;
using FluxStore.Api.Domain;
using FluxStore.Api.Shared.Errors;
using System.Text.RegularExpressions;

namespace FluxStore.Api.Domain.UserAggregate.ValueObjects
{
    public sealed class PhoneNumber : ValueObject
    {
        private static readonly Regex E164Pattern = new(@"^\+[1-9]\d{1,14}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public string Value { get; }

        private PhoneNumber(string value)
        {
            Value = value;
        }

        public static Result<PhoneNumber> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !E164Pattern.IsMatch(value))
            {
                return Result<PhoneNumber>.Invalid(new[]
                {
                    new ValidationError
                    {
                        ErrorCode = ProfileErrors.InvalidPhone.Code,
                        ErrorMessage = ProfileErrors.InvalidPhone.Description,
                        Identifier = "Phone"
                    }
                });
            }

            return Result<PhoneNumber>.Success(new PhoneNumber(value));
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}
