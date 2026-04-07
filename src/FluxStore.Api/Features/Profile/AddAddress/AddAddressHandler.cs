using Ardalis.Result;
using FluxStore.Api.Domain.AddressAggregate;
using FluxStore.Api.Domain.AddressAggregate.ValueObjects;
using FluxStore.Api.Infrastructure.Persistence;
using FluxStore.Api.Infrastructure.Services;
using FluxStore.Api.Shared.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FluxStore.Api.Features.Profile.AddAddress
{
    public sealed class AddAddressHandler : IRequestHandler<AddAddressRequest, Result<AddAddressResponse>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentUser _currentUser;

        public AddAddressHandler(ApplicationDbContext dbContext, CurrentUser currentUser)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }

        public async Task<Result<AddAddressResponse>> Handle(AddAddressRequest request, CancellationToken cancellationToken)
        {
            var activeAddressesCount = await _dbContext.Addresses
                .AsNoTracking()
                .CountAsync(x => x.UserId == _currentUser.UserId && !x.IsDeleted, cancellationToken);

            if (activeAddressesCount >= 10)
            {
                IEnumerable<string> errorList = new List<string> { AddressErrors.LimitExceeded.Code, AddressErrors.LimitExceeded.Description };
                return Result<AddAddressResponse>.Error(new ErrorList(errorList));
            }

            var labelResult = AddressLabel.Create(request.Label.Trim());
            if (!labelResult.IsSuccess) return Result<AddAddressResponse>.Invalid(labelResult.ValidationErrors);

            var streetResult = Street.Create(request.Street.Trim());
            if (!streetResult.IsSuccess) return Result<AddAddressResponse>.Invalid(streetResult.ValidationErrors);

            var countryResult = Country.Create(request.Country.Trim());
            if (!countryResult.IsSuccess) return Result<AddAddressResponse>.Invalid(countryResult.ValidationErrors);

            var postalCodeResult = PostalCode.Create(request.PostalCode.Trim());
            if (!postalCodeResult.IsSuccess) return Result<AddAddressResponse>.Invalid(postalCodeResult.ValidationErrors);

            var isDefault = activeAddressesCount == 0;
            var createResult = Address.Create(
                Guid.NewGuid(),
                _currentUser.UserId,
                labelResult.Value,
                streetResult.Value,
                request.City.Trim(),
                string.IsNullOrWhiteSpace(request.State) ? null : request.State.Trim(),
                countryResult.Value,
                postalCodeResult.Value,
                isDefault);

            if (!createResult.IsSuccess)
            {
                return Result<AddAddressResponse>.Invalid(createResult.ValidationErrors);
            }

            _dbContext.Addresses.Add(createResult.Value);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var address = createResult.Value;
            return Result<AddAddressResponse>.Success(new AddAddressResponse(
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
