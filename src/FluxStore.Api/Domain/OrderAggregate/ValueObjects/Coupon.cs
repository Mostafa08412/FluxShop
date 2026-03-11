using Ardalis.Result;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Extensions;

namespace FluxStore.Api.Domain.OrderAggregate.ValueObjects
{
    public sealed class Coupon : ValueObject
    {
        public const int MaxCodeLength = 8;

        public string Code { get; }
        public decimal Percentage { get; }

        private Coupon(string code, decimal percentage)
        {
            Code = code;
            Percentage = percentage;
        }

        public static Result<Coupon> Create(string code, decimal percentage)
        {
            if (string.IsNullOrWhiteSpace(code))
                return Result<Coupon>.Invalid(OrderErrors.CouponCodeIsRequired.ToValidationError());

            if (code.Length > MaxCodeLength)
                return Result<Coupon>.Invalid(OrderErrors.CouponCodeTooLong.ToValidationError());

            if (percentage <= 0 || percentage > 1)
                return Result<Coupon>.Invalid(OrderErrors.CouponPercentageOutOfRange.ToValidationError());

            return Result<Coupon>.Success(new Coupon(code, percentage));
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Code;
            yield return Percentage;
        }
    }
}
