using Ardalis.Result;
using FluxStore.Api.Infrastructure.Persistence;
using FluxStore.Api.Infrastructure.Services;
using FluxStore.Api.Shared.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FluxStore.Api.Features.Profile.RemovePaymentMethod
{
    public sealed class RemovePaymentMethodHandler : IRequestHandler<RemovePaymentMethodRequest, Result<Unit>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentUser _currentUser;

        public RemovePaymentMethodHandler(ApplicationDbContext dbContext, CurrentUser currentUser)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }

        public async Task<Result<Unit>> Handle(RemovePaymentMethodRequest request, CancellationToken cancellationToken)
        {
            var paymentMethod = await _dbContext.PaymentMethods.FirstOrDefaultAsync(
                x => x.Id == request.Id && x.UserId == _currentUser.UserId,
                cancellationToken);

            if (paymentMethod is null)
            {
                return Result<Unit>.NotFound(PaymentErrors.NotFound.Code, PaymentErrors.NotFound.Description);
            }

            var removeResult = paymentMethod.Remove();
            if (!removeResult.IsSuccess)
            {
                return Result<Unit>.Invalid(removeResult.ValidationErrors);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result<Unit>.NoContent();
        }
    }
}