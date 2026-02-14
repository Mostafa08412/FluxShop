using FluxStore.Domain.Core.Primitives.Result;
using MediatR;

namespace FluxStore.Application.Auth.Register
{
    public record RegisterCommand : IRequest<Result>
    {
        public string Name { get; init; }
        public string EmailAddress { get; init; }
        public string Password { get; init; }

    }
}
