using Ardalis.Result;
using FluxStore.Api.Infrastructure.Persistence;
using FluxStore.Api.Infrastructure.Services;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FluxStore.Api.Features.Profile.DeleteAddress
{
    public sealed class DeleteAddressHandler : IRequestHandler<DeleteAddressRequest, Result<Unit>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentUser _currentUser;

        public DeleteAddressHandler(ApplicationDbContext dbContext, CurrentUser currentUser)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }

        public async Task<Result<Unit>> Handle(DeleteAddressRequest request, CancellationToken cancellationToken)
        {
            var address = await _dbContext.Addresses.FirstOrDefaultAsync(
                x => x.Id == request.Id && x.UserId == _currentUser.UserId,
                cancellationToken);

            if (address is null || address.UserId != _currentUser.UserId)
            {
                return Result<Unit>.NotFound(AddressErrors.NotFoundError);
            }


            var activeAddressCount = await _dbContext.Addresses.CountAsync(
                x => x.UserId == _currentUser.UserId && !x.IsDeleted,
                cancellationToken);

            if (address.IsDefault && activeAddressCount <= 1)
            {
                return Result<Unit>.Invalid(AddressErrors.CannotDeleteOnlyDefault.ToValidationError());
            }

            var deleteResult = address.Delete();
            if (!deleteResult.IsSuccess)
            {
                return Result<Unit>.Invalid(deleteResult.ValidationErrors);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result<Unit>.NoContent();
        }
    }
}
