using FluxStore.Application.Auth.Common;
using FluxStore.Domain.Core.Primitives.Result;
using MediatR;

namespace FluxStore.Application.Auth.RefreshToken
{
    public record RefreshTokenCommand : IRequest<Result<AuthenticationResponse>>
    {
        public string refreshToken { get; init; }
    }

}
