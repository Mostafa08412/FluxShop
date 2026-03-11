using Ardalis.Result;
using FluentAssertions;
using FluxStore.Api.Domain.OrderAggregate.ValueObjects;
using FluxStore.Api.Shared.Errors;
using Xunit;

namespace FluxStore.UnitTest.DomainTests.OrderTests.ValueObjects
{
    public class ShippingAddressTests
    {
        [Theory]
        [InlineData(null, "Doe", "Egypt", "Street 1", "Cairo", "Cairo", "11224", "01044444444")]
        [InlineData("", "Doe", "Egypt", "Street 1", "Cairo", "Cairo", "11224", "01044444444")]

        [InlineData("John", null, "Egypt", "Street 1", "Cairo", "Cairo", "11224", "01044444444")]
        [InlineData("John", "", "Egypt", "Street 1", "Cairo", "Cairo", "11224", "01044444444")]

        [InlineData("John", "Doe", null, "Street 1", "Cairo", "Cairo", "11224", "01044444444")]
        [InlineData("John", "Doe", "   ", "Street 1", "Cairo", "Cairo", "11224", "01044444444")]

        [InlineData("John", "Doe", "Egypt", null, "Cairo", "Cairo", "11224", "01044444444")]

        [InlineData("John", "Doe", "Egypt", "Street 1", null, "Cairo", "11224", "01044444444")]

        [InlineData("John", "Doe", "Egypt", "Street 1", "Cairo", "", "11224", "01044444444")]

        [InlineData("John", "Doe", "Egypt", "Street 1", "Cairo", "Cairo", null, "01044444444")]

        [InlineData("John", "Doe", "Egypt", "Street 1", "Cairo", "Cairo", "11224", null)]
        [InlineData("John", "Doe", "Egypt", "Street 1", "Cairo", "Cairo", "11224", " ")]

        public void Create_WhenAnyRequiredFieldIsEmpty_ShouldReturnFailure(
      string firstName, string lastName, string country,
      string streetName, string city, string state,
      string zipCode, string phoneNumber)
        {
            var result = ShippingAddress.Create(firstName, lastName, country, streetName, city, state, zipCode, phoneNumber);

            result.IsSuccess.Should().BeFalse();
            result.IsInvalid().Should().BeTrue();
            result.ValidationErrors.Should().NotBeEmpty();
        }


        [Theory]
        [InlineData("0101234567")]   // 10 digits
        [InlineData("010123456789")] // 12 digits
        [InlineData("01312345678")]
        [InlineData("01412345678")]
        [InlineData("02012345678")]
        [InlineData("0101234567a")]
        [InlineData("010-1234-567")]
        [InlineData("+201012345678")]
        public void Create_WhenPhoneNumberIsInvalid_ShouldReturnFailure(string number)
        {
            // Arrange & Act
            var result = ShippingAddress.Create("John", "Doe", "Egypt", "Street 1", "Cairo", "Cairo", "11224", number);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsInvalid().Should().BeTrue();
            result.ValidationErrors.Should().Contain(x => x.ErrorCode == OrderErrors.InvalidEgyptianPhoneNumber.Code);
        }

        [Theory]
        [InlineData("01012345678")]
        [InlineData("01112345678")]
        [InlineData("01212345678")]
        [InlineData("01512345678")]
        public void Create_WhenDataIsValid_ShouldReturnSuccess(string number)
        {
            // Arrange & Act
            var result = ShippingAddress.Create("John", "Doe", "Egypt", "Street 1", "Cairo", "Cairo", "11224", number);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.FirstName.Should().Be("John");
            result.Value.PhoneNumber.Should().Be(number);
        }

        [Fact]
        public void Equals_WhenObjectsHaveSameValues_ShouldReturnTrue()
        {
            // Arrange
            var address1 = ShippingAddress.Create("John", "Doe", "Egypt", "Street 1", "Cairo", "Cairo", "11224", "01012345678").Value;
            var address2 = ShippingAddress.Create("John", "Doe", "Egypt", "Street 1", "Cairo", "Cairo", "11224", "01012345678").Value;

            // Act & Assert
            address1.Equals(address2).Should().BeTrue();
            (address1 == address2).Should().BeTrue();
            address1.GetHashCode().Should().Be(address2.GetHashCode());
        }

        [Fact]
        public void Equals_WhenObjectsHaveDifferentValues_ShouldReturnFalse()
        {
            // Arrange
            var address1 = ShippingAddress.Create("John", "Doe", "Egypt", "Street 1", "Cairo", "Cairo", "11224", "01012345678").Value;
            var address2 = ShippingAddress.Create("Jane", "Doe", "Egypt", "Street 1", "Cairo", "Cairo", "11224", "01012345678").Value;

            // Act & Assert
            address1.Equals(address2).Should().BeFalse();
            (address1 == address2).Should().BeFalse();
            address1.GetHashCode().Should().NotBe(address2.GetHashCode());
        }

        [Fact]
        public void Equals_WhenComparingToNull_ShouldReturnFalse()
        {
            // Arrange
            var address = ShippingAddress.Create("John", "Doe", "Egypt", "Street 1", "Cairo", "Cairo", "11224", "01012345678").Value;

            // Act & Assert
            address.Equals(null).Should().BeFalse();
            (address == null).Should().BeFalse();
        }

        [Fact]
        public void Equals_WhenComparingToDifferentType_ShouldReturnFalse()
        {
            // Arrange
            var address = ShippingAddress.Create("John", "Doe", "Egypt", "Street 1", "Cairo", "Cairo", "11224", "01012345678").Value;
            var otherObject = new object();

            // Act & Assert
            address.Equals(otherObject).Should().BeFalse();
        }
    }
}
