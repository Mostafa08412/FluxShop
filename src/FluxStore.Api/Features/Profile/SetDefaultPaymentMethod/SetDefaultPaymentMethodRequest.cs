using Ardalis.Result;
using FastEndpoints;
using MediatR;

namespace FluxStore.Api.Features.Profile.SetDefaultPaymentMethod
{
    public sealed record SetDefaultPaymentMethodRequest : IRequest<Result<SetDefaultPaymentMethodResponse>>, Shared.Markers.ICommand
    {
        [BindFrom("id")]
        public Guid Id { get; init; }
    }
}