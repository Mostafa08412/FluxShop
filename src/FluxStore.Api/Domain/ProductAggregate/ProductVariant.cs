using Ardalis.Result;
using FluxStore.Api.Domain.Enums;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Extensions;

namespace FluxStore.Api.Domain.ProductAggregate;

public sealed class ProductVariant : Entity
{
    public string SKU { get; private set; }
    public string Color { get; private set; }
    public ProductVariantSize Size { get; private set; }
    public int QuantityOnHand { get; private set; }
    public int QuantityReserved { get; private set; }

    public int QuantityAvailable => QuantityOnHand - QuantityReserved;

    // Concurrency Token
    public byte[] RowVersion { get; private set; }

    internal ProductVariant(Guid id, string sku, string color, ProductVariantSize size, int stockQuantity) : base(id)
    {
        SKU = sku;
        Color = color;
        Size = size;
        QuantityOnHand = stockQuantity;
    }

    private ProductVariant() { } // For EF Core

    internal static Result<ProductVariant> Create(string sku, string color, ProductVariantSize size, int initialStock)
    {
        if (string.IsNullOrWhiteSpace(sku))
            return Result<ProductVariant>.Invalid(ProductErrors.SKUIsRequired.ToValidationError());
        if (string.IsNullOrWhiteSpace(color))
            return Result<ProductVariant>.Invalid(ProductErrors.VariantColorIsRequired.ToValidationError());
        if (!Enum.IsDefined(size) || size == ProductVariantSize.None)
            return Result<ProductVariant>.Invalid(ProductErrors.VariantSizeIsRequired.ToValidationError());
        if (initialStock < 0)
            return Result<ProductVariant>.Invalid(ProductErrors.InvalidStockQuantity.ToValidationError());

        return Result<ProductVariant>.Success(new ProductVariant(Guid.CreateVersion7(), sku, color, size, initialStock));
    }

    internal Result Restock(int quantity)
    {
        if (quantity <= 0)
            return Result.Invalid(ProductErrors.InvalidStockQuantityChange.ToValidationError());

        QuantityOnHand += quantity;
        return Result.Success();
    }
    internal Result AdjustStock(int newStock)
    {
        if (newStock <= 0)
            return Result.Invalid(ProductErrors.InvalidStockQuantity.ToValidationError());

        QuantityOnHand = newStock;
        return Result.Success();
    }
    internal Result CommitStock(int quantity)
    {
        if (quantity <= 0)
            return Result.Invalid(ProductErrors.InvalidStockQuantityChange.ToValidationError());

        if (QuantityOnHand < quantity)
            return Result.Error(new ErrorList(new[] { ProductErrors.InsufficientStock.Code, ProductErrors.InsufficientStock.Description }));

        QuantityOnHand -= quantity;
        return Result.Success();
    }

    //Release Stock
    //Reserve Stock
}
