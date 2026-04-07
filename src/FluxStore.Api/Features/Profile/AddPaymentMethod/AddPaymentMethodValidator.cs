using FluentValidation;
using FluxStore.Api.Shared.Errors;

namespace FluxStore.Api.Features.Profile.AddPaymentMethod
{
    public sealed class AddPaymentMethodValidator : AbstractValidator<AddPaymentMethodRequest>
    {
        public AddPaymentMethodValidator()
        {
            RuleFor(x => x.CardNumber)
                .NotEmpty()
                    .WithMessage(PaymentErrors.InvalidCard.Description)
                    .WithErrorCode(PaymentErrors.InvalidCard.Code)
                    .WithName(nameof(AddPaymentMethodRequest.CardNumber))
                .Matches(@"^\d{13,19}$")
                    .WithMessage(PaymentErrors.InvalidCard.Description)
                    .WithErrorCode(PaymentErrors.InvalidCard.Code)
                    .WithName(nameof(AddPaymentMethodRequest.CardNumber));

            RuleFor(x => x.ExpiryMonth)
                .InclusiveBetween(1, 12)
                    .WithMessage(PaymentErrors.InvalidExpiryMonth.Description)
                    .WithErrorCode(PaymentErrors.InvalidExpiryMonth.Code)
                    .WithName(nameof(AddPaymentMethodRequest.ExpiryMonth));

            RuleFor(x => x.ExpiryYear)
                .GreaterThanOrEqualTo(DateTime.UtcNow.Year)
                    .WithMessage(PaymentErrors.Expired.Description)
                    .WithErrorCode(PaymentErrors.Expired.Code)
                    .WithName(nameof(AddPaymentMethodRequest.ExpiryYear));

            RuleFor(x => x.Cvv)
                .NotEmpty()
                    .WithMessage(PaymentErrors.InvalidCvv.Description)
                    .WithErrorCode(PaymentErrors.InvalidCvv.Code)
                    .WithName(nameof(AddPaymentMethodRequest.Cvv))
                .Matches(@"^\d{3,4}$")
                    .WithMessage(PaymentErrors.InvalidCvv.Description)
                    .WithErrorCode(PaymentErrors.InvalidCvv.Code)
                    .WithName(nameof(AddPaymentMethodRequest.Cvv));
        }
    }
}