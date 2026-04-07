using FastEndpoints;
using FluxStore.Api.Extensions;
using FluxStore.Api.Shared.Extensions;
using FluxStore.Api.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace FluxStore.Api.Features.Profile.ListPaymentMethods
{
    [Authorize]
    public sealed class ListPaymentMethodsEndpoint : Endpoint<EmptyRequest, ApiResponse<ListPaymentMethodsResponse>>
    {
        private readonly IMediator _mediator;

        public ListPaymentMethodsEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get(ApiRoutes.Account.PaymentMethods);
            Group<ApiGroups.AccountGroup>();
        }

        public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
        {
            var finalResult = (await _mediator.Send(new ListPaymentMethodsRequest(), ct)).ToApiResponse(HttpContext);
            await Send.ResponseAsync(finalResult, finalResult.StatusCode, ct);
        }
    }
}