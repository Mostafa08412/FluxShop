using FastEndpoints;
using FluxStore.Api.Extensions;
using FluxStore.Api.Shared.Extensions;
using FluxStore.Api.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace FluxStore.Api.Features.Profile.RemovePaymentMethod
{
    [Authorize]
    public sealed class RemovePaymentMethodEndpoint : Endpoint<RemovePaymentMethodRequest, ApiResponse<Unit>>
    {
        private readonly IMediator _mediator;

        public RemovePaymentMethodEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Delete(ApiRoutes.Account.PaymentMethodById);
            Group<ApiGroups.AccountGroup>();
        }

        public override async Task HandleAsync(RemovePaymentMethodRequest req, CancellationToken ct)
        {
            var finalResult = (await _mediator.Send(req, ct)).ToApiResponse(HttpContext);
            await Send.ResponseAsync(finalResult, finalResult.StatusCode, ct);
        }
    }
}