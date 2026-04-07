using Ardalis.Result;
using FluxStore.Api.Domain.UserAggregate.ValueObjects;
using FluxStore.Api.Infrastructure.Persistence;
using FluxStore.Api.Infrastructure.Services;
using FluxStore.Api.Shared.Errors;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace FluxStore.Api.Features.Profile.GetProfile
{
    public sealed class GetProfileHandler : IRequestHandler<GetProfileRequest, Result<GetProfileResponse>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentUser _currentUser;

        public GetProfileHandler(ApplicationDbContext dbContext, CurrentUser currentUser)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }

        public async Task<Result<GetProfileResponse>> Handle(GetProfileRequest request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.DomainUsers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == _currentUser.UserId, cancellationToken);

            if (user is null)
            {
                return Result<GetProfileResponse>.NotFound(ProfileErrors.ProfileNotFound.Code, ProfileErrors.ProfileNotFound.Description);
            }

            return Result<GetProfileResponse>.Success(new GetProfileResponse(
                user.Id,
                user.DisplayName,
                user.Email,
                user.Phone?.Value,
                user.AvatarUrl));
        }
    }
}
