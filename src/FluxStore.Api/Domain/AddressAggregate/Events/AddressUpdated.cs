using FluxStore.Api.Domain.AddressAggregate.ValueObjects;
using FluxStore.Api.Shared.Abstractions;

namespace FluxStore.Api.Domain.AddressAggregate.Events
{
    public sealed record AddressUpdated(
        Guid AddressId,
        AddressLabel Label,
        Street Street,
        string City,
        string? State,
        Country Country,
        PostalCode PostalCode) : IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
    }
}
