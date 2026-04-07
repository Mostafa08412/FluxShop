using Ardalis.Result;
using FastEndpoints;
using MediatR;

namespace FluxStore.Api.Features.Profile.RemovePaymentMethod
{
    public sealed record RemovePaymentMethodRequest : IRequest<Result<Unit>>, Shared.Markers.ICommand
    {
        [BindFrom("id")]
        public Guid Id { get; init; }
    }
}