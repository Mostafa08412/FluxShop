using Ardalis.Result;
using FluxStore.Api.Shared.Markers;
using MediatR;

namespace FluxStore.Api.Features.Profile.SetDefaultAddress
{
    public sealed record SetDefaultAddressRequest : IRequest<Result<SetDefaultAddressResponse>>, ICommand
    {

        public Guid Id { get; init; }

    }
}
