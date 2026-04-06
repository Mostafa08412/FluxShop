using FluxStore.Api.Domain;

namespace FluxStore.Api.Shared.Errors;

public static class ProductErrors
{
    public static readonly Error NameIsRequired = new("Product.NameRequired", "Product name is required.", ErrorType.Validation);
    public static readonly Error InvalidPrice = new("Product.InvalidPrice", "Product price must be greater than zero.", ErrorType.Validation);
    public static readonly Error InvalidDiscount = new("Product.InvalidDiscount", "Product discount cannot be greater than the price or negative.", ErrorType.Validation);
    public static readonly Error CategoryIdIsRequired = new("Product.CategoryRequired", "Category ID is required for a product.", ErrorType.Validation);
    public static readonly Error ProductHasNoVariants = new("Product.NoVariants", "A product must have at least one variant to be published.", ErrorType.ConditionNotMet);
    public static readonly Error InvalidDiscountPercentage = new("Product.InvalidDiscountPercentage", "Discount percentage must be greater than 0 and less than or equal to 100.", ErrorType.Validation);
    public static readonly Error NoDiscountToRemove = new("Product.NoDiscountToRemove", "The product does not currently have an active discount to remove.", ErrorType.ConditionNotMet);
    
    // Variant errors
    public static readonly Error VariantColorIsRequired = new("ProductVariant.ColorRequired", "Variant color is required.", ErrorType.Validation);
    public static readonly Error VariantSizeIsRequired = new("ProductVariant.SizeRequired", "Variant size is required.", ErrorType.Validation);
    public static readonly Error InvalidStockQuantity = new("ProductVariant.InvalidStock", "Variant stock quantity cannot be negative.", ErrorType.Validation);
    public static readonly Error SKUIsRequired = new("ProductVariant.SKURequired", "Variant SKU is required.", ErrorType.Validation);
    public static readonly Error InvalidStockQuantityChange = new("ProductVariant.InvalidStockChange", "Quantity to add or remove must be positive.", ErrorType.Validation);
    public static readonly Error InsufficientStock = new("ProductVariant.InsufficientStock", "Insufficient stock available for the requested quantity.", ErrorType.ConditionNotMet);
    public static readonly Error DuplicateSKU = new("ProductVariant.DuplicateSKU", "A variant with this SKU already exists in the product.", ErrorType.Conflict);
    public static readonly Error VariantNotFound = new("ProductVariant.NotFound", "The specified variant was not found.", ErrorType.NotFound);
    public static readonly Error InvalidStarRating = new("Product.InvalidStarRating", "Star rating must be between 1 and 5.", ErrorType.Validation);

    // Image errors
    public static readonly Error ImageUrlIsRequired = new("ProductImage.UrlRequired", "Product image URL is required.", ErrorType.Validation);
}
