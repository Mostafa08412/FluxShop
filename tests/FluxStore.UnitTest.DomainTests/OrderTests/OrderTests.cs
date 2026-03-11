using FluentAssertions;
using FluxStore.Api.Domain.Enums;
using FluxStore.Api.Domain.OrderAggregate;
using FluxStore.Api.Domain.OrderAggregate.Events;
using FluxStore.Api.Domain.OrderAggregate.ValueObjects;
using FluxStore.Api.Shared.Errors;
using FluxStore.UnitTest.DomainTests.OrderTests.Builders;
using Xunit;

namespace FluxStore.UnitTest.DomainTests.OrderTests
{
    public class OrderTests
    {
        private readonly Guid _userId = Guid.NewGuid();
        private readonly OrderNumber _orderNumber = OrderNumber.Create("ORD-260310-0001").Value;
        private readonly ShippingAddress _shippingAddress = ShippingAddress.Create("John", "Doe", "Egypt", "Street 1", "Cairo", "Cairo", "11224", "01012345678").Value;
        private readonly List<OrderItem> _items;

        public OrderTests()
        {
            var productSnapshot = ProductSnapshotBuilder.Create().WithValidData().Build();
            _items = new List<OrderItem> { OrderItem.Create(productSnapshot, 2).Value };
        }

        [Fact]
        public void Create_WhenDataIsValid_ShouldReturnSuccess()
        {
            // Arrange & Act
            var result = Order.Create(_userId, _orderNumber, "Stripe", ShippingMethod.Normal, _shippingAddress, null, _items, 10m, 5m, DateTime.UtcNow);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var order = result.Value;
            order.Status.Should().Be(OrderStatus.Placed);
            order.Total.Should().Be(order.Subtotal + 10m - 5m);
            order.TrackingSteps.Should().ContainSingle(s => s.Status == TrackingStatus.OrderPlaced);
            order.DomainEvents.Should().ContainSingle(e => e is OrderPlacedEvent);
        }

        [Fact]
        public void Create_WhenShippingCostIsNegative_ShouldReturnFailure()
        {
            // Act
            var result = Order.Create(_userId, _orderNumber, "Stripe", ShippingMethod.Normal, _shippingAddress, null, _items, -1m, 5m, DateTime.UtcNow);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ValidationErrors.Should().ContainSingle(e => e.ErrorCode == OrderErrors.NegativeShippingCost.Code);
        }

        [Fact]
        public void Create_WhenDiscountIsNegative_ShouldReturnFailure()
        {
            // Act
            var result = Order.Create(_userId, _orderNumber, "Stripe", ShippingMethod.Normal, _shippingAddress, null, _items, 10m, -1m, DateTime.UtcNow);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ValidationErrors.Should().ContainSingle(e => e.ErrorCode == OrderErrors.NegativeDiscount.Code);
        }

        [Fact]
        public void Create_WhenTotalWouldBeNegative_ShouldReturnFailure()
        {
            // Act
            // Subtotal is 100 * 2 = 200 (based on ProductSnapshotBuilder default)
            // Let's pass a huge discount
            var result = Order.Create(_userId, _orderNumber, "Stripe", ShippingMethod.Normal, _shippingAddress, null, _items, 0m, 3000m, DateTime.UtcNow);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ValidationErrors.Should().ContainSingle(e => e.ErrorCode == OrderErrors.NegativeTotal.Code);
        }

        [Fact]
        public void Lifecycle_ShouldTransitionCorrectly()
        {
            // Arrange
            var order = Order.Create(_userId, _orderNumber, "Stripe", ShippingMethod.Normal, _shippingAddress, null, _items, 10m, 5m, DateTime.UtcNow).Value;
            var paymentInfo = PaymentInfo.Create(Guid.NewGuid(), DateTime.UtcNow).Value;

            // Act: Confirm
            var confirmResult = order.Confirm(paymentInfo, DateTime.UtcNow);
            confirmResult.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Confirmed);
            order.TrackingSteps.Should().ContainSingle(s => s.Status == TrackingStatus.Confirmed);

            // Act: StartPreprocessing
            var processResult = order.StartProcessing(DateTime.UtcNow);
            processResult.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Processing);
            order.TrackingSteps.Should().Contain(s => s.Status == TrackingStatus.Processing);

            // Act: Ship
            var shipResult = order.Ship("TRACK-123", DateTime.UtcNow);
            shipResult.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Shipped);
            order.TrackingNumber.Should().Be("TRACK-123");
            order.TrackingSteps.Should().Contain(s => s.Status == TrackingStatus.Shipped);

            // Act: Deliver
            var deliverResult = order.Deliver(DateTime.UtcNow);
            deliverResult.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Delivered);
            order.TrackingSteps.Should().Contain(s => s.Status == TrackingStatus.Delivered);
        }

        [Fact]
        public void Confirm_WhenNotPlaced_ShouldReturnFailure()
        {
            // Arrange
            var order = Order.Create(_userId, _orderNumber, "Stripe", ShippingMethod.Normal, _shippingAddress, null, _items, 10m, 5m, DateTime.UtcNow).Value;
            var paymentInfo = PaymentInfo.Create(Guid.NewGuid(), DateTime.UtcNow).Value;
            order.Confirm(paymentInfo, DateTime.UtcNow);

            // Act
            var result = order.Confirm(paymentInfo, DateTime.UtcNow);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(OrderErrors.CannotConfirmOrder.Description);
        }

        [Fact]
        public void Ship_WhenPlaced_ShouldReturnFailure()
        {
            // Arrange
            var order = Order.Create(_userId, _orderNumber, "Stripe", ShippingMethod.Normal, _shippingAddress, null, _items, 10m, 5m, DateTime.UtcNow).Value;

            // Act
            var result = order.Ship("TRACK-123", DateTime.UtcNow);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(OrderErrors.CannotShipOrder.Description);
        }

        [Fact]
        public void Ship_WhenConfirmed_ShouldReturnSuccess()
        {
            // Arrange
            var order = Order.Create(_userId, _orderNumber, "Stripe", ShippingMethod.Normal, _shippingAddress, null, _items, 10m, 5m, DateTime.UtcNow).Value;
            var paymentInfo = PaymentInfo.Create(Guid.NewGuid(), DateTime.UtcNow).Value;
            order.Confirm(paymentInfo, DateTime.UtcNow);

            order.StartProcessing(DateTime.UtcNow, "");

            // Act
            var result = order.Ship("TRACK-123", DateTime.UtcNow);

            // Assert
            result.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Shipped);
        }

        [Fact]
        public void Ship_WhenPreprocessing_ShouldReturnSuccess()
        {
            // Arrange
            var order = Order.Create(_userId, _orderNumber, "Stripe", ShippingMethod.Normal, _shippingAddress, null, _items, 10m, 5m, DateTime.UtcNow).Value;
            var paymentInfo = PaymentInfo.Create(Guid.NewGuid(), DateTime.UtcNow).Value;
            order.Confirm(paymentInfo, DateTime.UtcNow);
            order.StartProcessing(DateTime.UtcNow);

            // Act
            var result = order.Ship("TRACK-123", DateTime.UtcNow);

            // Assert
            result.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Shipped);
        }

        [Fact]
        public void IssueRefund_WhenStatusIsValid_ShouldReturnSuccess()
        {
            // Arrange
            var order = Order.Create(_userId, _orderNumber, "Stripe", ShippingMethod.Normal, _shippingAddress, null, _items, 10m, 5m, DateTime.UtcNow).Value;
            var paymentInfo = PaymentInfo.Create(Guid.NewGuid(), DateTime.UtcNow).Value;
            order.Confirm(paymentInfo, DateTime.UtcNow);
            order.Cancel("", DateTime.UtcNow);

            // Act
            var result = order.IssueRefund(DateTime.UtcNow);

            // Assert
            result.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Refunded);
            order.TrackingSteps.Should().Contain(s => s.Status == TrackingStatus.Refunded);
        }
    }
}
