using Ardalis.Result;
using FluxStore.Api.Domain.Enums;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Extensions;

namespace FluxStore.Api.Domain.OrderAggregate.ValueObjects
{
    public sealed class TrackingStep : ValueObject
    {
        public TrackingStatus Status { get; }
        public string Description { get; }
        public DateTime OccurredAt { get; }

        private TrackingStep(TrackingStatus status, string description, DateTime occurredAt)
        {
            Status = status;
            Description = description;
            OccurredAt = occurredAt;
        }

        public static Result<TrackingStep> Create(
            TrackingStatus status, string description, DateTime occurredAt)
        {
            if (string.IsNullOrWhiteSpace(description))
                return Result<TrackingStep>.Invalid(
                    OrderErrors.TrackingStepDescriptionIsRequired.ToValidationError());

            return Result<TrackingStep>.Success(
                new TrackingStep(status, description, occurredAt));
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Status;
            yield return Description;
            yield return OccurredAt;
        }
    }
}
