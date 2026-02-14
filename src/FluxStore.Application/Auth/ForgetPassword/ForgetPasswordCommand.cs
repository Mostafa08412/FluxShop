using FluxStore.Domain.Core.Primitives.Result;
using MediatR;

namespace FluxStore.Application.Auth.ForgetPassword
{
    public record ForgetPasswordCommand : IRequest<Result>
    {
        public string EmailAddress { get; init; }
    }
}
