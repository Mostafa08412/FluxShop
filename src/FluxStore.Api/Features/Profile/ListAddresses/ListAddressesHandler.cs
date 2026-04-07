using Ardalis.Result;
using FluxStore.Api.Infrastructure.Persistence;
using FluxStore.Api.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FluxStore.Api.Features.Profile.ListAddresses
{
    public sealed class ListAddressesHandler : IRequestHandler<ListAddressesRequest, Result<ListAddressesResponse>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentUser _currentUser;

        public ListAddressesHandler(ApplicationDbContext dbContext, CurrentUser currentUser)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }

        public async Task<Result<ListAddressesResponse>> Handle(ListAddressesRequest request, CancellationToken cancellationToken)
        {
            var addresses = await _dbContext.Addresses
                .AsNoTracking()
                .Where(x => x.UserId == _currentUser.UserId)
                .OrderByDescending(x => x.IsDefault)
                .ThenBy(x => x.CreatedAt)
                .Select(x => new AddressDto(
                    x.Id,
                    x.Label.Value,
                    x.Street.Value,
                    x.City,
                    x.State,
                    x.Country.Name,
                    x.PostalCode.Value,
                    x.IsDefault))
                .ToListAsync(cancellationToken);


            return Result<ListAddressesResponse>.Success(new ListAddressesResponse(addresses));
        }
    }
}
