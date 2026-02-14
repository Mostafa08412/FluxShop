using FluxStore.Application.Auth.Common;
using FluxStore.Domain.Core.Primitives.Result;
using MediatR;

namespace FluxStore.Application.Auth.GoogleLogin
{
    public record GoogleLoginCommand : IRequest<Result<AuthenticationResponse>>
    {
        public string IdToken { get; init; }
    }
}
