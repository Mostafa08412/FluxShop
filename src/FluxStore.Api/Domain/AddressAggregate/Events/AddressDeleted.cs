using FluxStore.Api.Shared.Abstractions;

namespace FluxStore.Api.Domain.AddressAggregate.Events
{
    public sealed record AddressDeleted(Guid AddressId) : IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
    }
}
