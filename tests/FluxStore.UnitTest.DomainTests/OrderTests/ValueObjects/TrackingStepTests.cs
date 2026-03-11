using FluentAssertions;
using FluxStore.Api.Domain.Enums;
using FluxStore.Api.Domain.OrderAggregate.ValueObjects;
using FluxStore.Api.Shared.Errors;
using Xunit;

namespace FluxStore.UnitTest.DomainTests.OrderTests.ValueObjects
{
    public class TrackingStepTests
    {
        [Fact]
        public void Create_WhenDataIsValid_ShouldReturnSuccess()
        {
            // Arrange
            var now = DateTime.UtcNow;

            // Act
            var result = TrackingStep.Create(TrackingStatus.Shipped, "Far away", now);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Status.Should().Be(TrackingStatus.Shipped);
            result.Value.OccurredAt.Should().Be(now);
        }

        [Fact]
        public void Create_WhenDescriptionIsEmpty_ShouldReturnFailure()
        {
            // Act
            var result = TrackingStep.Create(TrackingStatus.Shipped, "", DateTime.UtcNow);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ValidationErrors.Should().Contain(x => x.ErrorCode == OrderErrors.TrackingStepDescriptionIsRequired.Code);
        }

        [Fact]
        public void Equals_WhenValuesAreSame_ShouldReturnTrue()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var s1 = TrackingStep.Create(TrackingStatus.Shipped, "Desc", now).Value;
            var s2 = TrackingStep.Create(TrackingStatus.Shipped, "Desc", now).Value;

            // Act & Assert
            s1.Equals(s2).Should().BeTrue();
            s1.GetHashCode().Should().Be(s2.GetHashCode());
        }
    }
}
