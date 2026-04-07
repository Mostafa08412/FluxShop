using FastEndpoints;
using FluxStore.Api.Extensions;
using FluxStore.Api.Shared.Extensions;
using FluxStore.Api.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace FluxStore.Api.Features.Profile.SetDefaultPaymentMethod
{
    [Authorize]
    public sealed class SetDefaultPaymentMethodEndpoint : Endpoint<SetDefaultPaymentMethodRequest, ApiResponse<SetDefaultPaymentMethodResponse>>
    {
        private readonly IMediator _mediator;

        public SetDefaultPaymentMethodEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Put(ApiRoutes.Account.PaymentMethodDefault);
            Group<ApiGroups.AccountGroup>();
            Version(2);
        }

        public override async Task HandleAsync(SetDefaultPaymentMethodRequest req, CancellationToken ct)
        {
            var finalResult = (await _mediator.Send(req, ct)).ToApiResponse(HttpContext);
            await Send.ResponseAsync(finalResult, finalResult.StatusCode, ct);
        }
    }
}