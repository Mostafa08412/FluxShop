using Ardalis.Result;
using FluxStore.Api.Markers;
using MediatR;

namespace FluxStore.Api.Features.Profile.GetProfile
{
    public sealed record GetProfileRequest : IRequest<Result<GetProfileResponse>>, IQuery;
}
