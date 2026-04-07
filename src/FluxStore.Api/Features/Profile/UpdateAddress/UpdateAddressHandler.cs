using Ardalis.Result;
using FluxStore.Api.Domain.AddressAggregate.ValueObjects;
using FluxStore.Api.Infrastructure.Persistence;
using FluxStore.Api.Infrastructure.Services;
using FluxStore.Api.Shared.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FluxStore.Api.Features.Profile.UpdateAddress
{
    public sealed class UpdateAddressHandler : IRequestHandler<UpdateAddressRequest, Result<UpdateAddressResponse>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentUser _currentUser;

        public UpdateAddressHandler(ApplicationDbContext dbContext, CurrentUser currentUser)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }

        public async Task<Result<UpdateAddressResponse>> Handle(UpdateAddressRequest request, CancellationToken cancellationToken)
        {
            var address = await _dbContext.Addresses.FirstOrDefaultAsync(
                x => x.Id == request.Id && x.UserId == _currentUser.UserId,
                cancellationToken);

            if (address is null)
            {
                return Result<UpdateAddressResponse>.NotFound(AddressErrors.NotFoundError);
            }

            var labelResult = AddressLabel.Create(request.Label.Trim());
            if (!labelResult.IsSuccess) return Result<UpdateAddressResponse>.Invalid(labelResult.ValidationErrors);

            var streetResult = Street.Create(request.Street.Trim());
            if (!streetResult.IsSuccess) return Result<UpdateAddressResponse>.Invalid(streetResult.ValidationErrors);

            var countryResult = Country.Create(request.Country.Trim());
            if (!countryResult.IsSuccess) return Result<UpdateAddressResponse>.Invalid(countryResult.ValidationErrors);

            var postalCodeResult = PostalCode.Create(request.PostalCode.Trim());
            if (!postalCodeResult.IsSuccess) return Result<UpdateAddressResponse>.Invalid(postalCodeResult.ValidationErrors);

            var updateResult = address.Update(
                labelResult.Value,
                streetResult.Value,
                request.City.Trim(),
                string.IsNullOrWhiteSpace(request.State) ? null : request.State.Trim(),
                countryResult.Value,
                postalCodeResult.Value);

            if (!updateResult.IsSuccess)
            {
                return Result<UpdateAddressResponse>.Invalid(updateResult.ValidationErrors);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<UpdateAddressResponse>.Success(new UpdateAddressResponse(
                address.Id,
                address.Label.Value,
                address.Street.Value,
                address.City,
                address.State,
                address.Country.Name,
                address.PostalCode.Value,
                address.IsDefault));
        }
    }
}
