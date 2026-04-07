using FluentValidation;
using FluxStore.Api.Shared.Errors;

namespace FluxStore.Api.Features.Profile.RemovePaymentMethod
{
    public sealed class RemovePaymentMethodValidator : AbstractValidator<RemovePaymentMethodRequest>
    {
        public RemovePaymentMethodValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                    .WithMessage(PaymentErrors.PaymentMethodIdRequired.Description)
                    .WithErrorCode(PaymentErrors.PaymentMethodIdRequired.Code)
                    .WithName(nameof(RemovePaymentMethodRequest.Id));
        }
    }
}