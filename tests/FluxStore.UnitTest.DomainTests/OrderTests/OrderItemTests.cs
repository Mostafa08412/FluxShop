using Ardalis.Result;
using FluentAssertions;
using FluxStore.Api.Domain.OrderAggregate;
using FluxStore.Api.Shared.Errors;
using FluxStore.UnitTest.DomainTests.OrderTests.Builders;
using Xunit;

namespace FluxStore.UnitTest.DomainTests.OrderTests
{
    public class OrderItemTests
    {


        [Fact]
        public void Create_WhenProductSnapshotIsNull_ReturnResultFailure()
        {
            var productSnapshot = ProductSnapshotBuilder.Create().WithInValidData().Build();

            var result = OrderItem.Create(productSnapshot, 1);

            result.IsSuccess.Should().BeFalse();

            result.IsInvalid().Should().BeTrue();

            result.ValidationErrors.Should().ContainSingle(X => X.ErrorCode == OrderErrors.ProductSnapshotIsRequired.Code);

        }

        [Fact]
        public void Create_WhenQuantityIsBelowOne_ReturnResultFailure()
        {
            var productSnapshot = ProductSnapshotBuilder.Create().WithValidData().Build();

            var result = OrderItem.Create(productSnapshot, -90);

            result.IsSuccess.Should().BeFalse();

            result.IsInvalid().Should().BeTrue();

            result.Value.Should().BeNull();

            result.ValidationErrors.Should().ContainSingle(X => X.ErrorCode == OrderErrors.QuantityMustBeAtLeastOne.Code);

        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(50)]
        [InlineData(1000)]
        public void Create_WhenQuantityIsOneOrMore_ReturnResultSuccess(int quantity)
        {
            var productSnapshot = ProductSnapshotBuilder.Create().WithValidData().Build();

            var result = OrderItem.Create(productSnapshot, quantity);

            result.IsSuccess.Should().BeTrue();

            result.Value.ProductSnapshot.Should().Be(productSnapshot);

            result.Value.Quantity.Should().Be(quantity);

            result.ValidationErrors.Should().BeEmpty();

            result.Errors.Should().BeEmpty();

        }

        [Fact]
        public void Equals_WhenValuesAreSame_ShouldReturnFailure()
        {
            var productSnapshot = ProductSnapshotBuilder.Create().WithValidData().Build();

            var order_item_1_result = OrderItem.Create(productSnapshot, 5);

            var order_item_2_result = OrderItem.Create(productSnapshot, 5);

            (order_item_1_result == order_item_2_result).Should().BeFalse();

            (order_item_1_result.Equals(order_item_2_result)).Should().BeFalse();
        }



    }
}
