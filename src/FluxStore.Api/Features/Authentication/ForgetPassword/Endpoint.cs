using Ardalis.Result;
using FastEndpoints;
using FluxStore.Api.Extensions;
using FluxStore.Api.Shared.Extensions;
using FluxStore.Api.Shared.Models;
using MediatR;
using static FluxStore.Api.Extensions.ApiGroups;

namespace FluxStore.Api.Features.Authentication.ForgetPassword
{
    public record ForgetPasswordRequest(string EmailAddress) : IRequest<Result<Unit>>;


    public class Endpoint : Endpoint<ForgetPasswordRequest, ApiResponse<Unit>>
    {
        private readonly IMediator _mediatr;

        public Endpoint(IMediator mediatr)
        {
            _mediatr = mediatr;
        }

        public override void Configure()
        {
            Post(ApiRoutes.ForgetPassword);
            Group<AuthenticationV1Group>();
            AllowAnonymous();
        }
        public override async Task HandleAsync(ForgetPasswordRequest req, CancellationToken ct)
        {
            var result = (await _mediatr.Send(req, ct)).ToApiResponse(HttpContext, statusCodeOnSuccess: 202);

            await Send.ResponseAsync(result, result.StatusCode, ct);
        }
    }












}
