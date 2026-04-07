using Ardalis.Result;
using FluxStore.Api.Shared.Markers;
using MediatR;

namespace FluxStore.Api.Features.Profile.AddPaymentMethod
{
    public sealed record AddPaymentMethodRequest(
        string CardNumber,
        int ExpiryMonth,
        int ExpiryYear,
        string Cvv) : IRequest<Result<AddPaymentMethodResponse>>, ICommand;
}