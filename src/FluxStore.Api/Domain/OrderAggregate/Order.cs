using Ardalis.Result;
using FluxStore.Api.Domain.Enums;
using FluxStore.Api.Domain.OrderAggregate.Events;
using FluxStore.Api.Domain.OrderAggregate.ValueObjects;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Extensions;

namespace FluxStore.Api.Domain.OrderAggregate
{
    public sealed class Order : Aggregate
    {
        public Guid UserId { get; private set; }
        public OrderNumber OrderNumber { get; private set; }
        public OrderStatus Status { get; private set; }
        public string PaymentProvider { get; private set; }
        public ShippingMethod ShippingMethod { get; private set; }
        public ShippingAddress ShippingAddress { get; private set; }
        public PaymentInfo? PaymentInfo { get; private set; }
        public Coupon? CouponCode { get; private set; }

        public decimal ShippingCost { get; private set; }
        public decimal Discount { get; private set; }

        public decimal Subtotal { get; private set; }
        public decimal Total { get; private set; }
        public string? TrackingNumber { get; private set; }
        public DateTime PlacedAt { get; private set; }

        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        private readonly List<TrackingStep> _trackingSteps = new();
        public IReadOnlyCollection<TrackingStep> TrackingSteps => _trackingSteps.AsReadOnly();


        // For EF Core
        private Order() { }

        private Order(
            Guid id,
            Guid userId,
            OrderNumber orderNumber,
            string paymentProvider,
            ShippingMethod shippingMethod,
            ShippingAddress shippingAddress,
            Coupon? coupon,
            List<OrderItem> items,
            decimal shippingCost,
            decimal discountAmount,
            DateTime placedAt) : base(id)
        {
            UserId = userId;
            OrderNumber = orderNumber;
            Status = OrderStatus.Placed;
            PaymentProvider = paymentProvider;
            ShippingMethod = shippingMethod;
            ShippingAddress = shippingAddress;
            CouponCode = coupon;
            PlacedAt = placedAt;

            _items.AddRange(items);

            ShippingCost = shippingCost;
            Subtotal = CalculateSubtotal();
            Discount = discountAmount;
            Total = CalculateTotal();
        }


        // ────────────────────────────────────────
        //  Factory
        // ────────────────────────────────────────

        public static Result<Order> Create(
            Guid userId,
            OrderNumber orderNumber,
            string paymentProvider,
            ShippingMethod shippingMethod,
            ShippingAddress shippingAddress,
            Coupon? coupon,
            List<OrderItem> items,
            decimal shippingCost,
            decimal discountAmount,
            DateTime placedAt)
        {
            if (userId == Guid.Empty)
                return Result<Order>.Invalid(OrderErrors.UserIdIsRequired.ToValidationError());

            if (orderNumber is null)
                return Result<Order>.Invalid(OrderErrors.OrderNumberIsRequired.ToValidationError());

            if (string.IsNullOrWhiteSpace(paymentProvider))
                return Result<Order>.Invalid(OrderErrors.PaymentProviderIsRequired.ToValidationError());

            if (shippingAddress is null)
                return Result<Order>.Invalid(OrderErrors.ShippingAddressIsRequired.ToValidationError());

            if (items is null || !items.Any())
                return Result<Order>.Invalid(OrderErrors.AtLeastOneItemRequired.ToValidationError());

            if (shippingCost < 0)
                return Result<Order>.Invalid(OrderErrors.NegativeShippingCost.ToValidationError());

            if (discountAmount < 0)
                return Result<Order>.Invalid(OrderErrors.NegativeDiscount.ToValidationError());

            var order = new Order(
                Guid.CreateVersion7(),
                userId,
                orderNumber!,
                paymentProvider,
                shippingMethod,
                shippingAddress!,
                coupon,
                items!,
                shippingCost,
                discountAmount,
                placedAt);

            if (order.Total < 0)
                return Result<Order>.Invalid(OrderErrors.NegativeTotal.ToValidationError());

            order.RaiseDomainEvent(new OrderPlacedEvent(
                order.Id,
                order.UserId,
                order.Total,
                order.PaymentProvider));

            order.AddTrackingStepInternal(TrackingStatus.OrderPlaced, "Order has been placed.", placedAt);

            return Result<Order>.Success(order);
        }


        // ────────────────────────────────────────
        //  State Transitions
        // ────────────────────────────────────────

        public Result Confirm(PaymentInfo paymentInfo, DateTime dateTime)
        {
            var validationResult = CanConfirm();

            if (!validationResult.IsSuccess)

                return validationResult;

            Status = OrderStatus.Confirmed;

            PaymentInfo = paymentInfo;

            RaiseDomainEvent(new OrderConfirmedEvent(Id, paymentInfo.PaymentId));

            AddTrackingStepInternal(TrackingStatus.Confirmed, "Order confirmed and moved to next step.", dateTime);

            return Result.Success();
        }

        public Result StartProcessing(DateTime occurredAt, string description = "Order is being processed.")
        {
            var validationResult = CanStartProcessing();
            if (!validationResult.IsSuccess)
                return validationResult;
            var result = AddTrackingStepInternal(TrackingStatus.Processing, description, occurredAt);

            if (!result.IsSuccess)
                return result;

            Status = OrderStatus.Processing;

            RaiseDomainEvent(new OrderProcessingEvent(Id));

            return result;
        }
        public Result Ship(string trackingNumber, DateTime occurredAt, string description = "Order has been shipped.")
        {
            var validationResult = CanShip(trackingNumber);

            if (!validationResult.IsSuccess)
                return validationResult;

            var trackingResult = AddTrackingStepInternal(TrackingStatus.Shipped, description, occurredAt);

            if (!trackingResult.IsSuccess)
                return trackingResult;

            Status = OrderStatus.Shipped;

            TrackingNumber = trackingNumber;

            RaiseDomainEvent(new OrderShippedEvent(Id, trackingNumber));
            return Result.Success();
        }
        public Result MarkAsOutForDelivery(DateTime occurredAt, string description = "Order is out for delivery.")
        {
            var validationResult = CanMarkAsOutForDelivery();
            if (!validationResult.IsSuccess)
                return validationResult;

            return AddTrackingStepInternal(TrackingStatus.OutForDelivery, description, occurredAt);
        }
        public Result Deliver(DateTime deliveredAt)
        {
            var validationResult = CanDeliver();
            if (!validationResult.IsSuccess)
                return validationResult;

            Status = OrderStatus.Delivered;

            RaiseDomainEvent(new OrderDeliveredEvent(Id, deliveredAt));

            AddTrackingStepInternal(TrackingStatus.Delivered, "Order has been delivered successfully.", deliveredAt);

            return Result.Success();
        }
        public Result Cancel(string reason, DateTime cancelledAt)
        {
            var validationResult = CanCancel();
            if (!validationResult.IsSuccess)
                return validationResult;

            Status = OrderStatus.Cancelled;

            bool isPaid = PaymentInfo != null;

            RaiseDomainEvent(new OrderCancelledEvent(Id, reason, isPaid, cancelledAt));

            AddTrackingStepInternal(TrackingStatus.Cancelled, $"Order cancelled: {reason}", cancelledAt);

            return Result.Success();
        }
        public Result IssueRefund(DateTime refundedAt)
        {
            var validationResult = CanIssueRefund();
            if (!validationResult.IsSuccess)
                return validationResult;

            Status = OrderStatus.Refunded;

            RaiseDomainEvent(new OrderRefundedEvent(Id, refundedAt));

            AddTrackingStepInternal(TrackingStatus.Refunded, "Order refund has been issued.", refundedAt);

            return Result.Success();
        }



        // ────────────────────────────────────────
        //  Private Methods
        // ────────────────────────────────────────

        private Result CanConfirm()
        {
            if (Status != OrderStatus.Placed)
                return Result.Error(new ErrorList([OrderErrors.CannotConfirmOrder.Code, OrderErrors.CannotConfirmOrder.Description]));

            return Result.Success();
        }

        private Result CanStartProcessing()
        {
            if (Status != OrderStatus.Confirmed)
                return Result.Error(new ErrorList([OrderErrors.CannotProcessOrder.Code, OrderErrors.CannotProcessOrder.Description]));

            return Result.Success();
        }

        private Result CanShip(string trackingNumber)
        {
            if (Status != OrderStatus.Processing)
                return Result.Error(new ErrorList([OrderErrors.CannotShipOrder.Code, OrderErrors.CannotShipOrder.Description]));

            if (string.IsNullOrWhiteSpace(trackingNumber))
                return Result.Error(new ErrorList([OrderErrors.TrackingNumberIsRequired.Code, OrderErrors.TrackingNumberIsRequired.Description]));

            return Result.Success();
        }

        private Result CanMarkAsOutForDelivery()
        {
            if (Status != OrderStatus.Shipped)
                return Result.Error(new ErrorList([OrderErrors.CannotMarkAsOutForDelivery.Code, OrderErrors.CannotMarkAsOutForDelivery.Description]));

            return Result.Success();
        }

        private Result CanDeliver()
        {
            if (Status != OrderStatus.Shipped)
                return Result.Error(new ErrorList([OrderErrors.CannotDeliverOrder.Code, OrderErrors.CannotDeliverOrder.Description]));

            return Result.Success();
        }

        private Result CanCancel()
        {
            if (Status is not OrderStatus.Placed && Status is not OrderStatus.Confirmed && Status is not OrderStatus.Processing)
                return Result.Error(new ErrorList([OrderErrors.CannotCancelOrder.Code, OrderErrors.CannotCancelOrder.Description]));

            return Result.Success();
        }

        private Result CanIssueRefund()
        {
            if (PaymentInfo == null)
                return Result.Error(new ErrorList([OrderErrors.CannotRefundUnpaidOrder.Code, OrderErrors.CannotRefundUnpaidOrder.Description]));

            if (Status is not OrderStatus.Cancelled && Status is not OrderStatus.Delivered)
                return Result.Error(new ErrorList([OrderErrors.CannotRefundOrder.Code, OrderErrors.CannotRefundOrder.Description]));

            return Result.Success();
        }


        private Result AddTrackingStepInternal(TrackingStatus status, string description, DateTime occurredAt)
        {
            var stepResult = TrackingStep.Create(status, description, occurredAt);
            if (!stepResult.IsSuccess)
            {
                return Result.Error(new ErrorList(stepResult.Errors.ToArray()));
            }

            if (_trackingSteps.Any(s => s.Status == status))
                return Result.Error(new ErrorList([OrderErrors.DuplicateTrackingStep.Code, OrderErrors.DuplicateTrackingStep.Description]));

            _trackingSteps.Add(stepResult.Value);

            RaiseDomainEvent(new TrackingStepAddedEvent(Id, status, description));

            return Result.Success();
        }

        private decimal CalculateSubtotal() =>
            _items.Sum(item => item.Quantity * item.ProductSnapshot.UnitPrice);

        private decimal CalculateTotal() =>
            Subtotal + ShippingCost - Discount;

    }
}
