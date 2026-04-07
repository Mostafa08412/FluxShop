using FluxStore.Api.Domain;

namespace FluxStore.Api.Shared.Errors
{
    public static class PaymentErrors
    {
        public static Error NotFound =>
            new("Payment_NotFound__PaymentMethodId", "Payment method not found.", ErrorType.NotFound);

        public static Error PaymentMethodIdRequired =>
            new("Payment_PaymentMethodIdRequired__PaymentMethodId", "Payment method id is required.", ErrorType.Validation);

        public static Error UserIdRequired =>
            new("Payment_UserIdRequired__UserId", "User id is required.", ErrorType.Validation);

        public static Error Last4Required =>
            new("Payment_Last4Required__Last4", "Last 4 digits are required.", ErrorType.Validation);

        public static Error GatewayTokenRequired =>
            new("Payment_GatewayTokenRequired__GatewayToken", "Gateway token is required.", ErrorType.Validation);

        public static Error InvalidBrand =>
            new("Payment_InvalidBrand__Brand", "Card brand is invalid.", ErrorType.Validation);

        public static Error InvalidExpiryMonth =>
            new("Payment_InvalidExpiryMonth__ExpiryMonth", "Expiry month must be between 1 and 12.", ErrorType.Validation);

        public static Error InvalidCvv =>
            new("Payment_InvalidCvv__Cvv", "CVV must contain 3 or 4 digits.", ErrorType.Validation);

        public static Error ExpiryRequired =>
            new("Payment_ExpiryRequired__ExpiryYear", "Card expiry is required.", ErrorType.Validation);

        public static Error LimitExceeded =>
            new("Payment_LimitExceeded__PaymentMethods", "Maximum 5 payment methods allowed.", ErrorType.Failure);

        public static Error TokenizationFailed =>
            new("Payment_TokenizationFailed__CardNumber", "Unable to process card. Please try again.", ErrorType.Failure);

        public static Error InvalidCard =>
            new("Payment_InvalidCard__CardNumber", "Card number failed validation.", ErrorType.Validation);

        public static Error Expired =>
            new("Payment_Expired__ExpiryYear", "Card expiry date is in the past.", ErrorType.Validation);
    }
}
