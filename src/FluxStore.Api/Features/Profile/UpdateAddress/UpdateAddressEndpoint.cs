using FastEndpoints;
using FluxStore.Api.Extensions;
using FluxStore.Api.Shared.Extensions;
using FluxStore.Api.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace FluxStore.Api.Features.Profile.UpdateAddress
{
    [Authorize]

    public sealed class UpdateAddressEndpoint : Endpoint<UpdateAddressRequest, ApiResponse<UpdateAddressResponse>>
    {
        private readonly IMediator _mediator;

        public UpdateAddressEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Put(ApiRoutes.AddressById);

            Group<ApiGroups.AccountV1Group>();
        }

        public override async Task HandleAsync(UpdateAddressRequest req, CancellationToken ct)
        {
            var finalResult = (await _mediator.Send(req, ct)).ToApiResponse(HttpContext);
            await Send.ResponseAsync(finalResult, finalResult.StatusCode, ct);
        }
    }
}
