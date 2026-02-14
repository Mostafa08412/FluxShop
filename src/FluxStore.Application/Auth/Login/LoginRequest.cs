using FluxStore.Application.Auth.Common;
using FluxStore.Domain.Core.Primitives.Result;
using MediatR;

namespace FluxStore.Application.Auth.Login
{
    public record LoginRequest : IRequest<Result<AuthenticationResponse>>
    {
        public required string EmailAddress { get; init; }
        public required string Password { get; init; }
    }

}

