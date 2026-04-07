using Ardalis.Result;
using FluxStore.Api.Infrastructure.Persistence;
using FluxStore.Api.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FluxStore.Api.Features.Profile.ListPaymentMethods
{
    public sealed class ListPaymentMethodsHandler : IRequestHandler<ListPaymentMethodsRequest, Result<ListPaymentMethodsResponse>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentUser _currentUser;

        public ListPaymentMethodsHandler(ApplicationDbContext dbContext, CurrentUser currentUser)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }

        public async Task<Result<ListPaymentMethodsResponse>> Handle(ListPaymentMethodsRequest request, CancellationToken cancellationToken)
        {
            var paymentMethods = await _dbContext.PaymentMethods
                .AsNoTracking()
                .Where(x => x.UserId == _currentUser.UserId && !x.IsDeleted)
                .OrderByDescending(x => x.IsDefault)
                .ThenBy(x => x.CreatedAt)
                .Select(x => new PaymentMethodDto(
                    x.Id,
                    x.Last4.Value,
                    x.Brand,
                    x.ExpiryMonth,
                    x.ExpiryYear,
                    x.IsDefault))
                .ToListAsync(cancellationToken);

            return Result<ListPaymentMethodsResponse>.Success(new ListPaymentMethodsResponse(paymentMethods));
        }
    }
}