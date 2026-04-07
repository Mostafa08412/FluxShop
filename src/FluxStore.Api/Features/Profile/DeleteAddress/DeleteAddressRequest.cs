using Ardalis.Result;
using FastEndpoints;
using MediatR;

namespace FluxStore.Api.Features.Profile.DeleteAddress
{
    public sealed record DeleteAddressRequest : IRequest<Result<Unit>>, Shared.Markers.ICommand
    {
        [BindFrom("id")]
        public Guid Id { get; init; }
    }
}
