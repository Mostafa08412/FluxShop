using FluxStore.Domain.Core.Primitives.Result;
using MediatR;

namespace FluxStore.Application.Users.Commands.DeactivateUser;

public record DeactivateUserCommand(string UserId) : IRequest<Result>;
