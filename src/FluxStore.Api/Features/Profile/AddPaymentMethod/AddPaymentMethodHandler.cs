using Ardalis.Result;
using FluxStore.Api.Domain;
using FluxStore.Api.Domain.PaymentMethodAggregate;
using FluxStore.Api.Domain.PaymentMethodAggregate.ValueObjects;
using FluxStore.Api.Infrastructure.Persistence;
using FluxStore.Api.Infrastructure.Services;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FluxStore.Api.Features.Profile.AddPaymentMethod
{
    public sealed class AddPaymentMethodHandler : IRequestHandler<AddPaymentMethodRequest, Result<AddPaymentMethodResponse>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentUser _currentUser;
        private readonly IPaymentGateway _paymentGateway;

        public AddPaymentMethodHandler(ApplicationDbContext dbContext, CurrentUser currentUser, IPaymentGateway paymentGateway)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
            _paymentGateway = paymentGateway;
        }

        public async Task<Result<AddPaymentMethodResponse>> Handle(AddPaymentMethodRequest request, CancellationToken cancellationToken)
        {
            var activeCount = await _dbContext.PaymentMethods.CountAsync(
                x => x.UserId == _currentUser.UserId && !x.IsDeleted,
                cancellationToken);

            if (activeCount >= 5)
            {
                return Result<AddPaymentMethodResponse>.Conflict(PaymentErrors.LimitExceeded.Code, PaymentErrors.LimitExceeded.Description);
            }

            var tokenResult = await TokenizeWithRetryAsync(request, cancellationToken);
            if (!tokenResult.IsSuccess)
            {
                return Result<AddPaymentMethodResponse>.Error(new ErrorList([PaymentErrors.TokenizationFailed.Code, PaymentErrors.TokenizationFailed.Description]));
            }

            var last4Result = Last4Digits.Create(request.CardNumber[^4..]);
            if (!last4Result.IsSuccess)
            {
                return Result<AddPaymentMethodResponse>.Invalid(last4Result.ValidationErrors);
            }

            var expiryResult = CardExpiry.Create(request.ExpiryMonth, request.ExpiryYear);
            if (!expiryResult.IsSuccess)
            {
                return Result<AddPaymentMethodResponse>.Invalid(expiryResult.ValidationErrors);
            }

            var paymentMethod = PaymentMethod.Create(
                Guid.NewGuid(),
                _currentUser.UserId,
                last4Result.Value,
                DetectBrand(request.CardNumber),
                expiryResult.Value,
                tokenResult.Value,
                activeCount == 0);

            if (!paymentMethod.IsSuccess)
            {
                return Result<AddPaymentMethodResponse>.Invalid(paymentMethod.ValidationErrors);
            }

            await _dbContext.PaymentMethods.AddAsync(paymentMethod.Value, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var response = Map(paymentMethod.Value);
            return Result<AddPaymentMethodResponse>.Success(response);
        }

        private async Task<Result<GatewayToken>> TokenizeWithRetryAsync(AddPaymentMethodRequest request, CancellationToken cancellationToken)
        {
            Result<GatewayToken> tokenResult = default!;

            for (var attempt = 1; attempt <= 3; attempt++)
            {
                tokenResult = await _paymentGateway.TokenizeCard(
                    request.CardNumber,
                    request.ExpiryMonth,
                    request.ExpiryYear,
                    request.Cvv,
                    cancellationToken);

                if (tokenResult.IsSuccess)
                {
                    return tokenResult;
                }
            }

            return Result<GatewayToken>.Error(new ErrorList([PaymentErrors.TokenizationFailed.Code, PaymentErrors.TokenizationFailed.Description]));
        }

        private static CardBrand DetectBrand(string cardNumber)
        {
            if (cardNumber.StartsWith("4", StringComparison.Ordinal))
            {
                return CardBrand.Visa;
            }

            if (cardNumber.StartsWith("34", StringComparison.Ordinal) || cardNumber.StartsWith("37", StringComparison.Ordinal))
            {
                return CardBrand.Amex;
            }

            if (cardNumber.StartsWith("6011", StringComparison.Ordinal) ||
                cardNumber.StartsWith("65", StringComparison.Ordinal) ||
                IsBetweenPrefixes(cardNumber, 644, 649) ||
                IsBetweenPrefixes(cardNumber, 622126, 622925))
            {
                return CardBrand.Discover;
            }

            if (IsBetweenPrefixes(cardNumber, 51, 55) || IsBetweenPrefixes(cardNumber, 2221, 2720))
            {
                return CardBrand.MasterCard;
            }

            return CardBrand.Other;
        }

        private static bool IsBetweenPrefixes(string cardNumber, int start, int end)
        {
            var length = start.ToString().Length;
            if (cardNumber.Length < length)
            {
                return false;
            }

            if (!int.TryParse(cardNumber[..length], out var prefix))
            {
                return false;
            }

            return prefix >= start && prefix <= end;
        }

        private static AddPaymentMethodResponse Map(PaymentMethod paymentMethod) =>
            new(paymentMethod.Id, paymentMethod.Last4.Value, paymentMethod.Brand, paymentMethod.ExpiryMonth, paymentMethod.ExpiryYear, paymentMethod.IsDefault);
    }
}