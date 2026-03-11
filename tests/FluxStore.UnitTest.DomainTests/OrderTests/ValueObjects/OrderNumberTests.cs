using Ardalis.Result;
using FluentAssertions;
using FluxStore.Api.Domain.OrderAggregate.ValueObjects;
using FluxStore.Api.Shared.Errors;
using Xunit;

namespace FluxStore.UnitTest.DomainTests.OrderTests.ValueObjects
{
    public class OrderNumberTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Create_WhenValueIsIsNullOrWhiteSpace_ShouldReturnFailure(string value)
        {
            // Act
            var result = OrderNumber.Create(value);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsInvalid().Should().BeTrue();
            result.ValidationErrors.Should().Contain(x => x.ErrorCode == OrderErrors.OrderNumberIsRequired.Code);
        }

        [Theory]
        [InlineData("ORD-123-4567")]
        [InlineData("ORD-123456-567")]
        [InlineData("123456-5678")]
        [InlineData("ORD1234567890")]
        public void Create_WhenFormatIsInvalid_ShouldReturnFailure(string value)
        {
            // Act
            var result = OrderNumber.Create(value);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsInvalid().Should().BeTrue();
            result.ValidationErrors.Should().Contain(x => x.ErrorCode == OrderErrors.InvalidOrderNumberFormat.Code);
        }

        [Fact]
        public void Create_WhenFormatIsValid_ShouldReturnSuccess()
        {
            // Arrange
            var value = "ORD-260308-0001";

            // Act
            var result = OrderNumber.Create(value);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be(value);
        }

        [Fact]
        public void Equals_WhenValuesAreSame_ShouldReturnTrue()
        {
            // Arrange
            var value = "ORD-260308-0001";
            var num1 = OrderNumber.Create(value).Value;
            var num2 = OrderNumber.Create(value).Value;

            // Act & Assert
            num1.Equals(num2).Should().BeTrue();
            (num1 == num2).Should().BeTrue();
            num1.GetHashCode().Should().Be(num2.GetHashCode());
        }

        [Fact]
        public void Equals_WhenValuesAreDifferent_ShouldReturnFalse()
        {
            // Arrange
            var num1 = OrderNumber.Create("ORD-260308-0001").Value;
            var num2 = OrderNumber.Create("ORD-260308-0002").Value;

            // Act & Assert
            num1.Equals(num2).Should().BeFalse();
            (num1 != num2).Should().BeTrue();
        }
    }
}
