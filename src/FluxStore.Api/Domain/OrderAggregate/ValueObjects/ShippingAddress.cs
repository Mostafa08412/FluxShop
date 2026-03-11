using System.Text.RegularExpressions;
using Ardalis.Result;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Extensions;

namespace FluxStore.Api.Domain.OrderAggregate.ValueObjects
{
    public sealed class ShippingAddress : ValueObject
    {
        private static readonly Regex EgyptianPhoneRegex = new(@"^01[0125]\d{8}$", RegexOptions.Compiled);

        public string FirstName { get; }
        public string LastName { get; }
        public string Country { get; }
        public string StreetName { get; }
        public string City { get; }
        public string State { get; }
        public string ZipCode { get; }
        public string PhoneNumber { get; }

        private ShippingAddress(
            string firstName, string lastName, string country,
            string streetName, string city, string state,
            string zipCode, string phoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            Country = country;
            StreetName = streetName;
            City = city;
            State = state;
            ZipCode = zipCode;
            PhoneNumber = phoneNumber;
        }

        public static Result<ShippingAddress> Create(
            string firstName, string lastName, string country,
            string streetName, string city, string state,
            string zipCode, string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                return Result<ShippingAddress>.Invalid(OrderErrors.ShippingFirstNameIsRequired.ToValidationError());
            if (string.IsNullOrWhiteSpace(lastName))
                return Result<ShippingAddress>.Invalid(OrderErrors.ShippingLastNameIsRequired.ToValidationError());
            if (string.IsNullOrWhiteSpace(country))
                return Result<ShippingAddress>.Invalid(OrderErrors.ShippingCountryIsRequired.ToValidationError());
            if (string.IsNullOrWhiteSpace(streetName))
                return Result<ShippingAddress>.Invalid(OrderErrors.ShippingStreetNameIsRequired.ToValidationError());
            if (string.IsNullOrWhiteSpace(city))
                return Result<ShippingAddress>.Invalid(OrderErrors.ShippingCityIsRequired.ToValidationError());
            if (string.IsNullOrWhiteSpace(state))
                return Result<ShippingAddress>.Invalid(OrderErrors.ShippingStateIsRequired.ToValidationError());
            if (string.IsNullOrWhiteSpace(zipCode))
                return Result<ShippingAddress>.Invalid(OrderErrors.ShippingZipCodeIsRequired.ToValidationError());

            if (string.IsNullOrWhiteSpace(phoneNumber))
                return Result<ShippingAddress>.Invalid(OrderErrors.ShippingPhoneNumberIsRequired.ToValidationError());
            
            if (!EgyptianPhoneRegex.IsMatch(phoneNumber))
                return Result<ShippingAddress>.Invalid(OrderErrors.InvalidEgyptianPhoneNumber.ToValidationError());

            return Result<ShippingAddress>.Success(
                new ShippingAddress(firstName, lastName, country,
                    streetName, city, state, zipCode, phoneNumber));
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return FirstName;
            yield return LastName;
            yield return Country;
            yield return StreetName;
            yield return City;
            yield return State;
            yield return ZipCode;
            yield return PhoneNumber;
        }
    }
}
