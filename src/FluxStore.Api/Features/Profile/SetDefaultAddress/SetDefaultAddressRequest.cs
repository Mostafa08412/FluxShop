using Ardalis.Result;
using MediatR;

namespace FluxStore.Api.Features.Profile.SetDefaultAddress
{
    public sealed record SetDefaultAddressRequest : IRequest<Result<SetDefaultAddressResponse>>, Markers.ICommand
    {

        public Guid Id { get; init; }

    }
}
