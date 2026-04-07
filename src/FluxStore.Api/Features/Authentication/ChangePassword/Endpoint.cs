using Ardalis.Result;
using FastEndpoints;
using FluxStore.Api.Extensions;
using FluxStore.Api.Shared.Extensions;
using FluxStore.Api.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using static FluxStore.Api.Extensions.ApiGroups;

namespace FluxStore.Api.Features.Authentication.ChangePassword
{
    public record ChangePasswordRequest(string CurrentPassword, string NewPassword) : IRequest<Result<Unit>>, Shared.Markers.ICommand;

    [Authorize]
    public class Endpoint : Endpoint<ChangePasswordRequest, ApiResponse<Unit>>
    {
        private readonly IMediator _mediator;

        public Endpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Post(ApiRoutes.Authentication.ChangePassword);

            Group<AuthenticationGroup>();
        }

        public override async Task HandleAsync(ChangePasswordRequest req, CancellationToken ct)
        {
            var result = await _mediator.Send(req, ct);
            var api = result.ToApiResponse(HttpContext);
            await Send.ResponseAsync(api, api.StatusCode, ct);
        }
    }
}
