using FluentValidation;
using FluxStore.Api.Shared.Errors;

namespace FluxStore.Api.Features.Profile.SetDefaultPaymentMethod
{
    public sealed class SetDefaultPaymentMethodValidator : AbstractValidator<SetDefaultPaymentMethodRequest>
    {
        public SetDefaultPaymentMethodValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                    .WithMessage(PaymentErrors.PaymentMethodIdRequired.Description)
                    .WithErrorCode(PaymentErrors.PaymentMethodIdRequired.Code)
                    .WithName(nameof(SetDefaultPaymentMethodRequest.Id));
        }
    }
}