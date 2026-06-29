using Ardalis.Result;
using FastEndpoints;
using MediatR;

namespace FluxStore.Api.Features.Profile.SetDefaultAddress
{
    public sealed record SetDefaultAddressRequest : IRequest<Result<SetDefaultAddressResponse>>, FastEndpoints.ICommand
    {
        [RouteParam]
        public Guid Id { get; init; }

    }
}
