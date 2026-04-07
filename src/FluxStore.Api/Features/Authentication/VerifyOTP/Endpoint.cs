using Ardalis.Result;
using FastEndpoints;
using FluxStore.Api.Extensions;
using FluxStore.Api.Resources;
using FluxStore.Api.Shared.Extensions;
using FluxStore.Api.Shared.Models;
using MediatR;
using Microsoft.Extensions.Localization;
using static FluxStore.Api.Extensions.ApiGroups;

namespace FluxStore.Api.Features.Authentication.VerifyOTP
{
    public record VerifyOTPRequest(string EmailAddress, string Otp) : IRequest<Result<VerifyOtpResponse>>, Shared.Markers.ICommand;
    public record VerifyOtpResponse(string ResetToken);

    public class Endpoint : Endpoint<VerifyOTPRequest, ApiResponse<VerifyOtpResponse>>
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
            Post(ApiRoutes.Authentication.VerifyOtp);
            Group<AuthenticationGroup>();
            Version(2);
            AllowAnonymous();
        }

        public override async Task HandleAsync(VerifyOTPRequest req, CancellationToken ct)
        {
            var result = await _mediator.Send(req, ct);

            var api = result.ToApiResponse(HttpContext, _localizer);

            await Send.ResponseAsync(api, api.StatusCode, ct);
        }
    }
}
