using Ardalis.Result;
using FluentAssertions;
using FluxStore.Api.Domain.OrderAggregate.ValueObjects;
using FluxStore.Api.Shared.Errors;
using Xunit;

namespace FluxStore.UnitTest.DomainTests.OrderTests.ValueObjects
{
    public class CouponTests
    {
        [Fact]
        public void Create_WhenDataIsValid_ShouldReturnSuccess()
        {
            // Act
            var result = Coupon.Create("SAVE10", 0.10m);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Code.Should().Be("SAVE10");
            result.Value.Percentage.Should().Be(0.10m);
        }

        [Fact]
        public void Create_WhenCodeTooLong_ShouldReturnFailure()
        {
            // Act
            var result = Coupon.Create("VERYLONGCODE", 0.10m);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ValidationErrors.Should().Contain(x => x.ErrorCode == OrderErrors.CouponCodeTooLong.Code);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1.1)]
        [InlineData(-0.1)]
        public void Create_WhenPercentageOutOfRange_ShouldReturnFailure(decimal percentage)
        {
            // Act
            var result = Coupon.Create("SAVE10", percentage);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ValidationErrors.Should().Contain(x => x.ErrorCode == OrderErrors.CouponPercentageOutOfRange.Code);
        }

        [Fact]
        public void Equals_WhenValuesAreSame_ShouldReturnTrue()
        {
            // Arrange
            var c1 = Coupon.Create("SAVE10", 0.10m).Value;
            var c2 = Coupon.Create("SAVE10", 0.10m).Value;

            // Act & Assert
            c1.Equals(c2).Should().BeTrue();
            c1.GetHashCode().Should().Be(c2.GetHashCode());
        }
    }
}
