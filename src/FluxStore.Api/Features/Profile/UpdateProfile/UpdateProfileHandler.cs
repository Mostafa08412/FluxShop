using Ardalis.Result;
using FluxStore.Api.Domain.UserAggregate.ValueObjects;
using FluxStore.Api.Infrastructure.Persistence;
using FluxStore.Api.Infrastructure.Services;
using FluxStore.Api.Shared.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FluxStore.Api.Features.Profile.UpdateProfile
{
    public sealed class UpdateProfileHandler : IRequestHandler<UpdateProfileRequest, Result<UpdateProfileResponse>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentUser _currentUser;

        public UpdateProfileHandler(ApplicationDbContext dbContext, CurrentUser currentUser)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }

        public async Task<Result<UpdateProfileResponse>> Handle(UpdateProfileRequest request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.DomainUsers.FirstOrDefaultAsync(x => x.Id == _currentUser.UserId, cancellationToken);

            if (user is null)
            {
                return Result<UpdateProfileResponse>.NotFound(ProfileErrors.ProfileNotFoundError);
            }

            PhoneNumber? phone = null;

            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                var phoneResult = PhoneNumber.Create(request.Phone);
                if (!phoneResult.IsSuccess)
                {
                    return Result<UpdateProfileResponse>.Invalid(phoneResult.ValidationErrors);
                }

                phone = phoneResult.Value;
            }

            var profileResult = user.UpdateProfile(request.DisplayName.Trim(), phone, request.AvatarUrl?.Trim());

            if (!profileResult.IsSuccess)
            {
                return Result<UpdateProfileResponse>.Invalid(profileResult.ValidationErrors);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<UpdateProfileResponse>.Success(new UpdateProfileResponse(
                user.Id,
                user.DisplayName,
                user.Email,
                user.Phone?.Value,
                user.AvatarUrl));
        }
    }
}
