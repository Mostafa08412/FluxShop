using Ardalis.Result;
using FastEndpoints;
using FluxStore.Api.Extensions;
using FluxStore.Api.Shared.Extensions;
using FluxStore.Api.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace FluxStore.Api.Features.Authentication.Logout
{

    public class LogoutRequest() : IRequest<Result<Unit>>, Markers.ICommand;


    [Authorize]
    public class Endpoint : Endpoint<EmptyRequest, ApiResponse<Unit>>
    {
        private readonly IMediator _mediator;

        public Endpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Post(ApiRoutes.Logout);
            Group<ApiGroups.AuthenticationV1Group>();

        }

        public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
        {

            var finalResult = await _mediator.Send(new LogoutRequest(), ct);
            var finalresult2 = finalResult.ToApiResponse(HttpContext);
            await Send.ResponseAsync(finalresult2, finalresult2.StatusCode, ct);
        }
    }
}
