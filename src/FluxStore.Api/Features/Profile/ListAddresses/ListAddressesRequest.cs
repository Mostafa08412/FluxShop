using Ardalis.Result;
using FluxStore.Api.Markers;
using MediatR;

namespace FluxStore.Api.Features.Profile.ListAddresses
{
    public sealed record ListAddressesRequest : IRequest<Result<ListAddressesResponse>>, IQuery;
}
