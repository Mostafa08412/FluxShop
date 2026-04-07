using Ardalis.Result;
using FluxStore.Api.Shared.Markers;
using MediatR;

namespace FluxStore.Api.Features.Profile.ListPaymentMethods
{
    public sealed record ListPaymentMethodsRequest() : IRequest<Result<ListPaymentMethodsResponse>>, IQuery;
}