using FluxStore.Domain.Core.Primitives.Result;
using MediatR;

namespace FluxStore.Application.Users.Commands.DeactivateUser;

public record DeactivateUserCommand(Guid UserId) : IRequest<Result>;
