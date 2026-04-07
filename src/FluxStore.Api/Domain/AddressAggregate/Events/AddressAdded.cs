using FluxStore.Api.Domain.AddressAggregate.ValueObjects;
using FluxStore.Api.Shared.Abstractions;

namespace FluxStore.Api.Domain.AddressAggregate.Events
{
    public sealed record AddressAdded(
        Guid AddressId,
        Guid UserId,
        AddressLabel Label,
        Street Street,
        string City,
        string? State,
        Country Country,
        PostalCode PostalCode,
        bool IsDefault) : IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
    }
}
