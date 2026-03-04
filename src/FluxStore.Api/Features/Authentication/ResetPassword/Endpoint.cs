using FastEndpoints;
using FluxStore.Api.Extensions;
using FluxStore.Api.Resources;
using FluxStore.Api.Shared.Extensions;
using FluxStore.Api.Shared.Models;
using MediatR;
using Microsoft.Extensions.Localization;
using static FluxStore.Api.Extensions.ApiGroups;

namespace FluxStore.Api.Features.Authentication.ResetPassword
{
    public class Endpoint : Endpoint<ResetPasswordRequest, ApiResponse<Unit>>
    {
        private readonly IMediator _mediator;

        private readonly IStringLocalizer<ErrorsResource> _localizer;

        public Endpoint(IMediator mediator, IStringLocalizer<ErrorsResource> localizer)
        {
            _mediator = mediator;
            _localizer = localizer;
        }

        public override void Configure()
        {
            Post(ApiRoutes.ResetPassword);
            Group<AuthenticationV1Group>();
            AllowAnonymous();
        }

        public override async Task HandleAsync(ResetPasswordRequest req, CancellationToken ct)
        {
            var result = await _mediator.Send(req, ct);

            var api = result.ToApiResponse(HttpContext, _localizer);

            await Send.ResponseAsync(api, api.StatusCode, ct);
        }
    }
}
