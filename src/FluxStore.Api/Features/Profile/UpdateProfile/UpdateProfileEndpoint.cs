using Ardalis.Result;
using FastEndpoints;
using FluxStore.Api.Extensions;
using FluxStore.Api.Shared.Extensions;
using FluxStore.Api.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace FluxStore.Api.Features.Profile.UpdateProfile
{
    [Authorize]
    public sealed class UpdateProfileEndpoint : Endpoint<UpdateProfileRequest, ApiResponse<UpdateProfileResponse>>
    {
        private readonly IMediator _mediator;

        public UpdateProfileEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Put(ApiRoutes.Profile);
            Group<ApiGroups.AccountV1Group>();
        }

        public override async Task HandleAsync(UpdateProfileRequest req, CancellationToken ct)
        {
            var finalResult = (await _mediator.Send(req, ct)).ToApiResponse(HttpContext);
            await Send.ResponseAsync(finalResult, finalResult.StatusCode, ct);
        }
    }
}
