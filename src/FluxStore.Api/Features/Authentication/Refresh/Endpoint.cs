using Ardalis.Result;
using FluxStore.Api.Extensions;
using FluxStore.Api.Markers;
using FluxStore.Api.Resources;
using FluxStore.Api.Shared.Extensions;
using FluxStore.Api.Shared.Models;
using MediatR;
using Microsoft.Extensions.Localization;

namespace FluxStore.Api.Features.Authentication.Refresh
{

    public record Endpoint(string RefreshToken) : IRequest<Result<RefreshTokenResponse>>, ICommand;
    public record RefreshTokenResponse(
        Guid UserId,
        string Email,
        string UserName,
        string FirstName,
        string LastName,
        IEnumerable<string> Roles,
        string AccessToken,
        string RefreshToken
    );
    public class RefreshTokenEndpoint : FastEndpoints.Endpoint<Endpoint, ApiResponse<RefreshTokenResponse>>
    {
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<ErrorsResource> stringLocalizer;

        public RefreshTokenEndpoint(IMediator mediator, IStringLocalizer<ErrorsResource> stringLocalizer)
        {
            _mediator = mediator;
            this.stringLocalizer = stringLocalizer;
        }

        public override void Configure()
        {
            Post(ApiRoutes.RefreshToken);
            Group<ApiGroups.AuthenticationV1Group>();
            AllowAnonymous();

        }

        public override async Task HandleAsync(Endpoint req, CancellationToken ct)
        {
            var result = (await _mediator.Send(req, ct)).ToApiResponse(HttpContext, stringLocalizer);
            await Send.ResponseAsync(result, result.StatusCode, ct);
        }

    }

}
