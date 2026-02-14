using FluxStore.Domain.Core.Primitives.Result;
using MediatR;

namespace FluxStore.Application.Users.Queries.GetUser;

public record GetUserDetailsQuery(string UserId) : IRequest<Result<UserDetailsDto>>;
