using FluxStore.Domain.Core.Primitives.Result;
using MediatR;

namespace FluxStore.Application.Auth.Logout
{
    public record LogoutCommand : IRequest<Result> { }
}
