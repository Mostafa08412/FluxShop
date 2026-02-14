using FluxStore.Domain.Core.Primitives.Result;
using MediatR;

namespace FluxStore.Application.Users.Commands.ActivateUser;

public record ActivateUserCommand(string UserId) : IRequest<Result>;
