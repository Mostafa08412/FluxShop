using Ardalis.Result;
using FluxStore.Api.Extensions;
using FluxStore.Api.Resources;
using FluxStore.Api.Shared.Extensions;
using FluxStore.Api.Shared.Models;
using MediatR;
using Microsoft.Extensions.Localization;

namespace FluxStore.Api.Features.Authentication.Login
{
    public class LoginRequest : IRequest<Result<LoginResponse>>, Markers.ICommand
    {
        public string EmailAddress { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
    public class LoginResponse
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
    public class Endpoint : FastEndpoints.Endpoint<LoginRequest, ApiResponse<LoginResponse>>
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
            Post(ApiRoutes.Login);
            Group<ApiGroups.AuthenticationV1Group>();
            AllowAnonymous();
        }

        public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
        {
            var finalResult = (await _mediator.Send(req, ct)).ToApiResponse(HttpContext, _localizer);
            await Send.ResponseAsync(finalResult, finalResult.StatusCode, ct);
        }
    }
}
