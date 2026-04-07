using Ardalis.Result;
using FluxStore.Api.Shared.Markers;
using MediatR;

namespace FluxStore.Api.Features.Profile.ListAddresses
{
    public sealed record ListAddressesRequest() : IRequest<Result<ListAddressesResponse>>, IQuery;
}
