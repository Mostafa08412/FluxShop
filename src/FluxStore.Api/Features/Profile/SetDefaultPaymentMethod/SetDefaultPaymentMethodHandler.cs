using Ardalis.Result;
using FluxStore.Api.Domain.PaymentMethodAggregate;
using FluxStore.Api.Infrastructure.Persistence;
using FluxStore.Api.Infrastructure.Services;
using FluxStore.Api.Shared.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FluxStore.Api.Features.Profile.SetDefaultPaymentMethod
{
    public sealed class SetDefaultPaymentMethodHandler : IRequestHandler<SetDefaultPaymentMethodRequest, Result<SetDefaultPaymentMethodResponse>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentUser _currentUser;

        public SetDefaultPaymentMethodHandler(ApplicationDbContext dbContext, CurrentUser currentUser)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }

        public async Task<Result<SetDefaultPaymentMethodResponse>> Handle(SetDefaultPaymentMethodRequest request, CancellationToken cancellationToken)
        {
            var target = await _dbContext.PaymentMethods.FirstOrDefaultAsync(
                x => x.Id == request.Id && x.UserId == _currentUser.UserId && !x.IsDeleted,
                cancellationToken);

            if (target is null)
            {
                return Result<SetDefaultPaymentMethodResponse>.NotFound(PaymentErrors.NotFound.Code, PaymentErrors.NotFound.Description);
            }

            if (target.IsDefault)
            {
                return Result<SetDefaultPaymentMethodResponse>.Success(Map(target));
            }

            var currentDefault = await _dbContext.PaymentMethods.FirstOrDefaultAsync(
                x => x.UserId == _currentUser.UserId && x.IsDefault && !x.IsDeleted,
                cancellationToken);

            if (currentDefault is not null && currentDefault.Id != target.Id)
            {
                var demoteResult = currentDefault.DemoteDefault();
                if (!demoteResult.IsSuccess)
                {
                    return Result<SetDefaultPaymentMethodResponse>.Invalid(demoteResult.ValidationErrors);
                }
            }

            var setResult = target.SetAsDefault();
            if (!setResult.IsSuccess)
            {
                return Result<SetDefaultPaymentMethodResponse>.Invalid(setResult.ValidationErrors);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result<SetDefaultPaymentMethodResponse>.Success(Map(target));
        }

        private static SetDefaultPaymentMethodResponse Map(PaymentMethod paymentMethod) =>
            new(paymentMethod.Id, paymentMethod.Last4.Value, paymentMethod.Brand, paymentMethod.ExpiryMonth, paymentMethod.ExpiryYear, paymentMethod.IsDefault);
    }
}