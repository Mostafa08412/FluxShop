using FastEndpoints;
using FluxStore.Api.Extensions;
using FluxStore.Api.Shared.Extensions;
using FluxStore.Api.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace FluxStore.Api.Features.Profile.SetDefaultAddress
{
    [Authorize]
    public sealed class SetDefaultAddressEndpoint : Endpoint<SetDefaultAddressRequest, ApiResponse<SetDefaultAddressResponse>>
    {
        private readonly IMediator _mediator;

        public SetDefaultAddressEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Put(ApiRoutes.AddressDefault);
            Group<ApiGroups.AccountV1Group>();
        }

        public override async Task HandleAsync(SetDefaultAddressRequest req, CancellationToken ct)
        {
            var finalResult = (await _mediator.Send(req, ct)).ToApiResponse(HttpContext);
            await Send.ResponseAsync(finalResult, finalResult.StatusCode, ct);
        }
    }
}
