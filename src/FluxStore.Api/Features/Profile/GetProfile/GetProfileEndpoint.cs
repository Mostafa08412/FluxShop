using FastEndpoints;
using FluxStore.Api.Extensions;
using FluxStore.Api.Shared.Extensions;
using FluxStore.Api.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace FluxStore.Api.Features.Profile.GetProfile
{
    [Authorize]
    public sealed class GetProfileEndpoint : Endpoint<EmptyRequest, ApiResponse<GetProfileResponse>>
    {
        private readonly IMediator _mediator;

        public GetProfileEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get(ApiRoutes.Account.Profile);
            Group<ApiGroups.AccountGroup>();
        }

        public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
        {
            var finalResult = (await _mediator.Send(new GetProfileRequest(), ct)).ToApiResponse(HttpContext);
            await Send.ResponseAsync(finalResult, finalResult.StatusCode, ct);
        }
    }
}
