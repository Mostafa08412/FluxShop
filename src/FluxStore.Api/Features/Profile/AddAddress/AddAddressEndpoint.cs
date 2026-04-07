using Ardalis.Result;
using FastEndpoints;
using FluxStore.Api.Extensions;
using FluxStore.Api.Shared.Extensions;
using FluxStore.Api.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace FluxStore.Api.Features.Profile.AddAddress
{
    [Authorize]
    public sealed class AddAddressEndpoint : Endpoint<AddAddressRequest, ApiResponse<AddAddressResponse>>
    {
        private readonly IMediator _mediator;

        public AddAddressEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Post(ApiRoutes.Addresses);
            Group<ApiGroups.AccountV1Group>();
        }

        public override async Task HandleAsync(AddAddressRequest req, CancellationToken ct)
        {
            var finalResult = (await _mediator.Send(req, ct)).ToApiResponse(HttpContext, statusCodeOnSuccess: 201);
            await Send.ResponseAsync(finalResult, finalResult.StatusCode, ct);
        }
    }
}
