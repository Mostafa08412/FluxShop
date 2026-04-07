using Ardalis.Result;
using FastEndpoints;
using FluxStore.Api.Extensions;
using FluxStore.Api.Resources;
using FluxStore.Api.Shared.Extensions;
using FluxStore.Api.Shared.Models;
using MediatR;
using Microsoft.Extensions.Localization;
using static FluxStore.Api.Extensions.ApiGroups;

namespace FluxStore.Api.Features.Authentication.GoogleLogin
{

    public record GoogleLoginRequest(string IdToken) : IRequest<Result<GoogleLoginResponse>>, Shared.Markers.ICommand;

    public class GoogleLoginResponse
    {
        public Guid UserId { get; init; } = Guid.Empty;
        public string Email { get; init; } = string.Empty;
        public string UserName { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public IEnumerable<string> Roles { get; init; } = Enumerable.Empty<string>();
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
    }

    public class Endpoint : Endpoint<GoogleLoginRequest, ApiResponse<GoogleLoginResponse>>
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
            Post(ApiRoutes.Authentication.LoginWithGoogle);
            Group<AuthenticationGroup>();
            AllowAnonymous();
        }
        public override async Task HandleAsync(GoogleLoginRequest req, CancellationToken ct)
        {
            var result = await _mediator.Send(req, ct);
            var apiResponse = result.ToApiResponse(HttpContext, _localizer);

            await Send.ResponseAsync(apiResponse, apiResponse.StatusCode, ct);

        }
    }
}