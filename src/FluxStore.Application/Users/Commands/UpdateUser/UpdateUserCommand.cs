using FluxStore.Domain.Core.Primitives.Result;
using MediatR;

namespace FluxStore.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string Role) : IRequest<Result>;
