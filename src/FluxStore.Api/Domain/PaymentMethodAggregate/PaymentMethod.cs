using Ardalis.Result;
using FluxStore.Api.Domain.PaymentMethodAggregate.Events;
using FluxStore.Api.Domain.PaymentMethodAggregate.ValueObjects;
using FluxStore.Api.Shared.Abstractions;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Extensions;

namespace FluxStore.Api.Domain.PaymentMethodAggregate
{
    public sealed class PaymentMethod : Aggregate
    {
        public Guid UserId { get; private set; }
        public Last4Digits Last4 { get; private set; } = null!;
        public CardBrand Brand { get; private set; }
        public int ExpiryMonth { get; private set; }
        public int ExpiryYear { get; private set; }
        public GatewayToken GatewayToken { get; private set; } = null!;
        public bool IsDefault { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset UpdatedAt { get; private set; }

        private PaymentMethod() { }

        private PaymentMethod(
            Guid id,
            Guid userId,
            Last4Digits last4,
            CardBrand brand,
            CardExpiry expiry,
            GatewayToken gatewayToken,
            bool isDefault,
            DateTimeOffset occurredOn) : base(id)
        {
            UserId = userId;
            Last4 = last4;
            Brand = brand;
            ExpiryMonth = expiry.Month;
            ExpiryYear = expiry.Year;
            GatewayToken = gatewayToken;
            IsDefault = isDefault;
            CreatedAt = occurredOn;
            UpdatedAt = occurredOn;
        }

        public static Result<PaymentMethod> Create(
            Guid id,
            Guid userId,
            Last4Digits last4,
            CardBrand brand,
            CardExpiry expiry,
            GatewayToken gatewayToken,
            bool isDefault = false,
            DateTimeOffset? occurredOn = null)
        {
            var errors = new List<Error>();

            if (id == Guid.Empty)
            {
                errors.Add(PaymentErrors.PaymentMethodIdRequired);
            }

            if (userId == Guid.Empty)
            {
                errors.Add(PaymentErrors.UserIdRequired);
            }

            if (last4 is null) errors.Add(PaymentErrors.Last4Required);
            if (gatewayToken is null) errors.Add(PaymentErrors.GatewayTokenRequired);
            if (!Enum.IsDefined(typeof(CardBrand), brand))
            {
                errors.Add(PaymentErrors.InvalidBrand);
            }

            if (expiry is null)
            {
                errors.Add(PaymentErrors.ExpiryRequired);
            }
            else if (expiry.IsExpired)
            {
                errors.Add(PaymentErrors.Expired);
            }

            if (errors.Any())
            {
                return Result<PaymentMethod>.Invalid(errors.Select(x => x.ToValidationError()));
            }

            var now = occurredOn ?? DateTimeOffset.UtcNow;
            var paymentMethod = new PaymentMethod(id, userId, last4!, brand, expiry!, gatewayToken!, isDefault, now);
            paymentMethod.RaiseDomainEvent(new PaymentMethodAdded(id, userId, last4!, brand, expiry!.Month, expiry!.Year, isDefault));
            return Result<PaymentMethod>.Success(paymentMethod);
        }

        public Result Remove()
        {
            if (IsDeleted)
            {
                return Result.Success();
            }

            IsDeleted = true;
            UpdatedAt = DateTimeOffset.UtcNow;
            RaiseDomainEvent(new PaymentMethodRemoved(Id));
            return Result.Success();
        }

        public Result SetAsDefault()
        {
            if (IsDeleted)
            {
                return Result.Invalid(PaymentErrors.NotFound.ToValidationError());
            }

            IsDefault = true;
            UpdatedAt = DateTimeOffset.UtcNow;
            RaiseDomainEvent(new DefaultPaymentMethodChanged(Id, UserId));
            return Result.Success();
        }

        public Result DemoteDefault()
        {
            IsDefault = false;
            UpdatedAt = DateTimeOffset.UtcNow;
            return Result.Success();
        }

        public void Apply(IDomainEvent domainEvent)
        {
            switch (domainEvent)
            {
                case PaymentMethodAdded added:
                    UserId = added.UserId;
                    Last4 = added.Last4;
                    Brand = added.Brand;
                    ExpiryMonth = added.ExpiryMonth;
                    ExpiryYear = added.ExpiryYear;
                    IsDefault = added.IsDefault;
                    CreatedAt = added.OccurredOn;
                    UpdatedAt = added.OccurredOn;
                    break;
                case PaymentMethodRemoved removed:
                    IsDeleted = true;
                    UpdatedAt = removed.OccurredOn;
                    break;
                case DefaultPaymentMethodChanged changed:
                    IsDefault = true;
                    UpdatedAt = changed.OccurredOn;
                    break;
            }
        }
    }
}
