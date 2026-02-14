using FluxStore.Domain.Core.Primitives.Result;
using MediatR;

namespace FluxStore.Application.Auth.ChangePassword
{
    public record ChangePasswordCommand : IRequest<Result>
    {
        public string CurrentPassword { get; init; }

        public string NewPassword { get; init; }

        public string ConfirmNewPassword { get; init; }

    }
}
