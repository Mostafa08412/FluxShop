using Ardalis.Result;
using FluxStore.Api.Extensions;
using FluxStore.Api.Markers;
using FluxStore.Api.Resources;
using FluxStore.Api.Shared.Extensions;
using FluxStore.Api.Shared.Models;
using MediatR;
using Microsoft.Extensions.Localization;

namespace FluxStore.Api.Features.Authentication.Register
{
    public class RegisterRequest : IRequest<Result<RegisterResponse>>, ICommand
    {
        public string EmailAddress { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string ConfirmPassword { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;

    }
    public class RegisterResponse
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
    public class RegisterUserEndpoint : FastEndpoints.Endpoint<RegisterRequest, ApiResponse<RegisterResponse>>
    {

        private readonly IMediator _mediator;
        private readonly IStringLocalizer<ErrorsResource> _localizer;


        public RegisterUserEndpoint(IMediator mediator, IStringLocalizer<ErrorsResource> localizer)
        {
            _mediator = mediator;
            _localizer = localizer;
        }

        public override void Configure()
        {
            Post(ApiRoutes.Register);
            Group<ApiGroups.AuthenticationV1Group>();
            AllowAnonymous();
        }

        public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
        {

            var finalResult = (await _mediator.Send(req, ct)).ToApiResponse(HttpContext, _localizer);
            await Send.ResponseAsync(finalResult, finalResult.StatusCode, ct);


        }
    }
}
