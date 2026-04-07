using FluxStore.Api.Shared.Abstractions;

namespace FluxStore.Api.Domain.AddressAggregate.Events
{
    public sealed record DefaultAddressChanged(
        Guid AddressId,
        Guid UserId) : IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
    }
}
