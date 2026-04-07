using Ardalis.Result;
using FluxStore.Api.Shared.Markers;
using MediatR;

namespace FluxStore.Api.Features.Profile.UpdateAddress
{
    public sealed record UpdateAddressRequest : IRequest<Result<UpdateAddressResponse>>, ICommand
    {
        public Guid Id { get; init; }
        public string Label { get; init; } = string.Empty;
        public string Street { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string? State { get; init; }
        public string Country { get; init; } = string.Empty;
        public string PostalCode { get; init; } = string.Empty;
    }
}
