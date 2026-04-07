using FastEndpoints;
using FluxStore.Api.Extensions;
using FluxStore.Api.Shared.Extensions;
using FluxStore.Api.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace FluxStore.Api.Features.Profile.AddPaymentMethod
{
    [Authorize]
    public sealed class AddPaymentMethodEndpoint : Endpoint<AddPaymentMethodRequest, ApiResponse<AddPaymentMethodResponse>>
    {
        private readonly IMediator _mediator;

        public AddPaymentMethodEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Post(ApiRoutes.Account.PaymentMethods);
            Group<ApiGroups.AccountGroup>();
        }

        public override async Task HandleAsync(AddPaymentMethodRequest req, CancellationToken ct)
        {
            var finalResult = (await _mediator.Send(req, ct)).ToApiResponse(HttpContext, statusCodeOnSuccess: StatusCodes.Status201Created);
            await Send.ResponseAsync(finalResult, finalResult.StatusCode, ct);
        }
    }
}