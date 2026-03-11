using FluxStore.Api.Domain;

namespace FluxStore.Api.Shared.Errors
{
    public static class OrderErrors
    {
        // --- Order Aggregate ---
        public static Error UserIdIsRequired =>
            new("Order_UserIdIsRequired__UserId", "User ID is required.", ErrorType.Validation);

        public static Error AtLeastOneItemRequired =>
            new("Order_AtLeastOneItemRequired__Items", "Order must have at least one item.", ErrorType.Validation);

        public static Error ShippingAddressIsRequired =>
            new("Order_ShippingAddressIsRequired__ShippingAddress", "Shipping address is required.", ErrorType.Validation);

        // --- Payment ---

        public static Error PaymentProviderIsRequired =>
         new("Order_PaymentProviderIsRequired__PaymentProvider", "The payment provider is required.", ErrorType.Validation);

        public static Error PaymentIdIsRequired =>
            new("Order_PaymentIdIsRequired__PaymentId", "Payment ID is required.", ErrorType.Validation);

        public static Error PaymentDateIsRequired =>
            new("Order_PaymentDateIsRequired__PaidAt", "Payment date is required.", ErrorType.Validation);

        public static Error NegativeTotal =>
            new("Order_NegativeTotal__Total", "Order total cannot be negative.", ErrorType.Validation);

        public static Error NegativeShippingCost =>
            new("Order_NegativeShippingCost__ShippingCost", "Shipping cost cannot be negative.", ErrorType.Validation);

        public static Error NegativeDiscount =>
            new("Order_NegativeDiscount__Discount", "Discount cannot be negative.", ErrorType.Validation);


        // --- State Transitions ---
        public static Error CannotConfirmOrder =>
            new("Order_CannotConfirmOrder__Status", "Order can only be confirmed from Placed status.", ErrorType.ConditionNotMet);

        public static Error CannotCancelOrder =>
            new("Order_CannotCancelOrder__Status", "Order can only be cancelled from Placed or Confirmed or Processing status.", ErrorType.ConditionNotMet);

        public static Error CannotDeliverOrder =>
            new("Order_CannotDeliverOrder__Status", "Order can only be delivered from Shipped status.", ErrorType.ConditionNotMet);

        public static Error CannotRefundOrder =>
            new("Order_CannotRefundOrder__Status", "Order can only be refunded from Cancelled or Delivered status.", ErrorType.ConditionNotMet);

        public static Error CannotRefundUnpaidOrder =>
            new("Order_CannotRefundUnpaidOrder__PaymentInfo", "Cannot issue a refund for an unpaid order.", ErrorType.ConditionNotMet);

        public static Error CannotProcessOrder =>
            new("Order_CannotProcessOrder__Status", "Order can only be processed from Confirmed status.", ErrorType.ConditionNotMet);

        public static Error CannotShipOrder =>
            new("Order_CannotShipOrder__Status", "Order can only be shipped from Processing status.", ErrorType.ConditionNotMet);

        public static Error CannotMarkAsOutForDelivery =>
            new("Order_CannotMarkAsOutForDelivery__Status", "Order can only be marked as out for delivery from Shipped status.", ErrorType.ConditionNotMet);

        public static Error TrackingNumberIsRequired =>
            new("Order_TrackingNumberIsRequired__TrackingNumber", "Tracking number is required when shipping the order.", ErrorType.Validation);

        // --- Tracking ---
        public static Error TrackingStepIsRequired =>
            new("Order_TrackingStepIsRequired__TrackingStep", "Tracking step is required.", ErrorType.Validation);

        // --- OrderNumber VO ---
        public static Error OrderNumberIsRequired =>
            new("Order_OrderNumberIsRequired__OrderNumber", "Order number value is required.", ErrorType.Validation);

        public static Error InvalidOrderNumberFormat =>
            new("Order_InvalidOrderNumberFormat__OrderNumber", "Order number must match the format ORD-YYMM-NNNN.", ErrorType.Validation);

        // --- ShippingAddress VO ---
        public static Error ShippingFirstNameIsRequired =>
            new("Order_ShippingFirstNameIsRequired__FirstName", "Shipping first name is required.", ErrorType.Validation);

        public static Error ShippingLastNameIsRequired =>
            new("Order_ShippingLastNameIsRequired__LastName", "Shipping last name is required.", ErrorType.Validation);

        public static Error ShippingCountryIsRequired =>
            new("Order_ShippingCountryIsRequired__Country", "Shipping country is required.", ErrorType.Validation);

        public static Error ShippingStreetNameIsRequired =>
            new("Order_ShippingStreetNameIsRequired__StreetName", "Shipping street name is required.", ErrorType.Validation);

        public static Error ShippingCityIsRequired =>
            new("Order_ShippingCityIsRequired__City", "Shipping city is required.", ErrorType.Validation);

        public static Error ShippingStateIsRequired =>
            new("Order_ShippingStateIsRequired__State", "Shipping state is required.", ErrorType.Validation);

        public static Error ShippingZipCodeIsRequired =>
            new("Order_ShippingZipCodeIsRequired__ZipCode", "Shipping zip code is required.", ErrorType.Validation);

        public static Error ShippingPhoneNumberIsRequired =>
            new("Order_ShippingPhoneNumberIsRequired__PhoneNumber", "Shipping phone number is required.", ErrorType.Validation);

        public static Error InvalidEgyptianPhoneNumber =>
            new("Order_InvalidEgyptianPhoneNumber__PhoneNumber", "Phone number must be a valid Egyptian number (e.g., 01XXXXXXXXX).", ErrorType.Validation);

        // --- CouponCode VO ---
        public static Error CouponCodeIsRequired =>
            new("Order_CouponCodeIsRequired__CouponCode", "Coupon code value is required.", ErrorType.Validation);

        public static Error CouponCodeTooLong =>
            new("Order_CouponCodeTooLong__CouponCode", "Coupon code must not exceed 8 characters.", ErrorType.Validation);

        public static Error CouponPercentageOutOfRange =>
            new("Order_CouponPercentageOutOfRange__Percentage", "Coupon percentage must be between 0 (exclusive) and 1 (inclusive).", ErrorType.Validation);

        // --- ProductSnapshot VO ---
        public static Error ProductIdIsRequired =>
            new("Order_ProductIdIsRequired__ProductId", "Product ID is required.", ErrorType.Validation);

        public static Error ProductVariantIdIsRequired =>
            new("Order_ProductVariantIdIsRequired__ProductVariantId", "Product variant ID is required.", ErrorType.Validation);

        public static Error ProductNameIsRequired =>
            new("Order_ProductNameIsRequired__ProductName", "Product name is required.", ErrorType.Validation);

        public static Error ProductVariantColorIsRequired =>
            new("Order_ProductVariantColorIsRequired__ProductVariantColor", "Product variant color is required.", ErrorType.Validation);

        public static Error InvalidProductVariantSize =>
            new("Order_InvalidProductVariantSize__ProductVariantSize", "Product variant size is invalid.", ErrorType.Validation);

        public static Error UnitPriceMustBePositive =>
            new("Order_UnitPriceMustBePositive__UnitPrice", "Unit price must be greater than zero.", ErrorType.Validation);

        // --- OrderItem Entity ---
        public static Error ProductSnapshotIsRequired =>
            new("Order_ProductSnapshotIsRequired__ProductSnapshot", "Product snapshot is required.", ErrorType.Validation);

        public static Error QuantityMustBeAtLeastOne =>
            new("Order_QuantityMustBeAtLeastOne__Quantity", "Quantity must be at least 1.", ErrorType.Validation);

        // --- TrackingStep VO ---
        public static Error TrackingStepDescriptionIsRequired =>
            new("Order_TrackingStepDescriptionIsRequired__Description", "Tracking step description is required.", ErrorType.Validation);

        public static Error DuplicateTrackingStep =>
            new("Order_DuplicateTrackingStep__Status", "This tracking status has already been recorded for this order.", ErrorType.Conflict);
    }
}
