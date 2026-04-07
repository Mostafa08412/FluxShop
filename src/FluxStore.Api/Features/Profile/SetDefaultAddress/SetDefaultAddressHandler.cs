using Ardalis.Result;
using FluxStore.Api.Infrastructure.Persistence;
using FluxStore.Api.Infrastructure.Services;
using FluxStore.Api.Shared.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FluxStore.Api.Features.Profile.SetDefaultAddress
{
    public sealed class SetDefaultAddressHandler : IRequestHandler<SetDefaultAddressRequest, Result<SetDefaultAddressResponse>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentUser _currentUser;

        public SetDefaultAddressHandler(ApplicationDbContext dbContext, CurrentUser currentUser)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }

        public async Task<Result<SetDefaultAddressResponse>> Handle(SetDefaultAddressRequest request, CancellationToken cancellationToken)
        {
            var target = await _dbContext.Addresses.FirstOrDefaultAsync(
                x => x.Id == request.Id && x.UserId == _currentUser.UserId,
                cancellationToken);

            if (target is null)
            {
                return Result<SetDefaultAddressResponse>.NotFound(AddressErrors.NotFoundError);
            }


            var currentDefault = await _dbContext.Addresses.FirstOrDefaultAsync(
                x => x.UserId == _currentUser.UserId && x.IsDefault,
                cancellationToken);

            if (currentDefault is not null && currentDefault.Id != target.Id)
            {
                currentDefault.DemoteDefault();
            }

            var setDefaultResult = target.SetAsDefault();

            if (!setDefaultResult.IsSuccess)
            {
                return Result<SetDefaultAddressResponse>.Invalid(setDefaultResult.ValidationErrors);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<SetDefaultAddressResponse>.Success(new SetDefaultAddressResponse(
                target.Id,
                target.Label.Value,
                target.Street.Value,
                target.City,
                target.State,
                target.Country.Name,
                target.PostalCode.Value,
                target.IsDefault));
        }
    }
}
