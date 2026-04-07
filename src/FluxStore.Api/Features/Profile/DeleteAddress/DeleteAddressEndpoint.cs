using FastEndpoints;
using FluxStore.Api.Extensions;
using FluxStore.Api.Shared.Extensions;
using FluxStore.Api.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace FluxStore.Api.Features.Profile.DeleteAddress
{
    [Authorize]
    public sealed class DeleteAddressEndpoint : Endpoint<DeleteAddressRequest, ApiResponse<Unit>>
    {
        private readonly IMediator _mediator;

        public DeleteAddressEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Delete(ApiRoutes.AddressById);
            Group<ApiGroups.AccountV1Group>();
        }

        public override async Task HandleAsync(DeleteAddressRequest req, CancellationToken ct)
        {
            var finalResult = (await _mediator.Send(req, ct)).ToApiResponse(HttpContext);
            await Send.ResponseAsync(finalResult, StatusCodes.Status200OK, ct);
        }
    }
}
