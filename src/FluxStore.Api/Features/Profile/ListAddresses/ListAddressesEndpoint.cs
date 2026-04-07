using FastEndpoints;
using FluxStore.Api.Extensions;
using FluxStore.Api.Shared.Extensions;
using FluxStore.Api.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace FluxStore.Api.Features.Profile.ListAddresses
{
    [Authorize]
    public sealed class ListAddressesEndpoint : Endpoint<EmptyRequest, ApiResponse<ListAddressesResponse>>
    {
        private readonly IMediator _mediator;

        public ListAddressesEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get(ApiRoutes.Addresses);
            Group<ApiGroups.AccountV1Group>();
        }

        public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
        {
            var finalResult = (await _mediator.Send(new ListAddressesRequest(), ct)).ToApiResponse(HttpContext);
            await Send.ResponseAsync(finalResult, finalResult.StatusCode, ct);
        }
    }
}
