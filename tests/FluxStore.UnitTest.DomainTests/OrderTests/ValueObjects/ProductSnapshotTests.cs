using Ardalis.Result;
using FluentAssertions;
using FluxStore.Api.Domain.Enums;
using FluxStore.Api.Domain.OrderAggregate.ValueObjects;
using FluxStore.Api.Shared.Errors;
using Xunit;

namespace FluxStore.UnitTest.DomainTests.OrderTests.ValueObjects
{
    public class ProductSnapshotTests
    {
        private readonly Guid _productId = Guid.NewGuid();
        private readonly Guid _variantId = Guid.NewGuid();
        private const string _name = "Test Product";
        private const string _color = "Red";
        private const ProductVariantSize _size = ProductVariantSize.XL;
        private const decimal _price = 100.00m;

        [Fact]
        public void Create_WhenDataIsValid_ShouldReturnSuccess()
        {
            // Act
            var result = ProductSnapshot.Create(_productId, _variantId, _name, _color, _size, _price);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.ProductId.Should().Be(_productId);
            result.Value.UnitPrice.Should().Be(_price);
        }

        [Fact]
        public void Create_WhenProductIdIsEmpty_ShouldReturnFailure()
        {
            // Act
            var result = ProductSnapshot.Create(Guid.Empty, _variantId, _name, _color, _size, _price);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ValidationErrors.Should().Contain(x => x.ErrorCode == OrderErrors.ProductIdIsRequired.Code);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Create_WhenUnitPriceIsNotPositive_ShouldReturnFailure(decimal price)
        {
            // Act
            var result = ProductSnapshot.Create(_productId, _variantId, _name, _color, _size, price);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ValidationErrors.Should().Contain(x => x.ErrorCode == OrderErrors.UnitPriceMustBePositive.Code);
        }

        [Fact]
        public void Equals_WhenValuesAreSame_ShouldReturnTrue()
        {
            // Arrange
            var s1 = ProductSnapshot.Create(_productId, _variantId, _name, _color, _size, _price).Value;
            var s2 = ProductSnapshot.Create(_productId, _variantId, _name, _color, _size, _price).Value;

            // Act & Assert
            s1.Equals(s2).Should().BeTrue();
            s1.GetHashCode().Should().Be(s2.GetHashCode());
        }
    }
}
