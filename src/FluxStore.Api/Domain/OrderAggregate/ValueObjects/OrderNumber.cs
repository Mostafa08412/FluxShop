using Ardalis.Result;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Extensions;
using System.Text.RegularExpressions;

namespace FluxStore.Api.Domain.OrderAggregate.ValueObjects
{
    public sealed class OrderNumber : ValueObject
    {
        private static readonly Regex FormatRegex = new(@"^ORD-\d{6}-\d{4}$", RegexOptions.Compiled);

        public string Value { get; }

        private OrderNumber(string value)
        {
            Value = value;
        }

        public static Result<OrderNumber> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result<OrderNumber>.Invalid(
                    OrderErrors.OrderNumberIsRequired.ToValidationError());

            if (!FormatRegex.IsMatch(value))
                return Result<OrderNumber>.Invalid(
                    OrderErrors.InvalidOrderNumberFormat.ToValidationError());

            return Result<OrderNumber>.Success(new OrderNumber(value));
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}
