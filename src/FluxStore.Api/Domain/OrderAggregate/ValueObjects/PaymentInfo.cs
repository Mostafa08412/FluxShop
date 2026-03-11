using Ardalis.Result;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Extensions;

namespace FluxStore.Api.Domain.OrderAggregate.ValueObjects
{
    public sealed class PaymentInfo : ValueObject
    {
        public Guid PaymentId { get; }
        public DateTime PaidAt { get; }

        private PaymentInfo(Guid paymentId, DateTime paidAt)
        {
            PaymentId = paymentId;
            PaidAt = paidAt;
        }

        public static Result<PaymentInfo> Create(Guid? paymentId, DateTime? paidAt)
        {
            if (paymentId == null || paymentId == Guid.Empty)
                return Result<PaymentInfo>.Invalid(OrderErrors.PaymentIdIsRequired.ToValidationError());

            if (paidAt == null)
                return Result<PaymentInfo>.Invalid(OrderErrors.PaymentDateIsRequired.ToValidationError());

            return Result<PaymentInfo>.Success(new PaymentInfo(paymentId.Value, paidAt.Value));
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return PaymentId;
            yield return PaidAt;
        }
    }
}
