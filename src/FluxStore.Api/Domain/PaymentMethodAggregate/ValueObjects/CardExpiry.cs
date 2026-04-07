using Ardalis.Result;
using FluentValidation.Results;
using FluxStore.Api.Domain;
using FluxStore.Api.Shared.Errors;

namespace FluxStore.Api.Domain.PaymentMethodAggregate.ValueObjects
{
    public sealed class CardExpiry : ValueObject
    {
        public int Month { get; }
        public int Year { get; }

        private CardExpiry(int month, int year)
        {
            Month = month;
            Year = year;
        }

        public static Result<CardExpiry> Create(int month, int year)
        {
            var currentYear = DateTime.UtcNow.Year;

            if (month < 1 || month > 12)
            {
                return Invalid("Payment_InvalidCard__ExpiryMonth", "Expiry month must be between 1 and 12.", "ExpiryMonth");
            }

            if (year < currentYear)
            {
                return Invalid(PaymentErrors.Expired.Code, PaymentErrors.Expired.Description, "ExpiryYear");
            }

            return Result<CardExpiry>.Success(new CardExpiry(month, year));
        }

        public bool IsExpired =>
            Year < DateTime.UtcNow.Year || (Year == DateTime.UtcNow.Year && Month < DateTime.UtcNow.Month);

        private static Result<CardExpiry> Invalid(string code, string message, string identifier) =>
            Result<CardExpiry>.Invalid(new[]
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
            yield return Month;
            yield return Year;
        }
    }
}
