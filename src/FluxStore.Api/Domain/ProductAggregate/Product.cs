using Ardalis.Result;
using FluxStore.Api.Domain.Enums;
using FluxStore.Api.Domain.ProductAggregate.Events;
using FluxStore.Api.Domain.ProductAggregate.ValueObjects;
using FluxStore.Api.Shared.Abstractions;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Extensions;

namespace FluxStore.Api.Domain.ProductAggregate;

public sealed class Product : Aggregate, ISoftDeletable
{
    public string Name { get; private set; }
    public string Description { get; private set; }

    // Explicit primitive instead of Price value object based on ADR update
    public decimal BasePrice { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    public decimal FinalPrice => BasePrice - DiscountAmount;

    public Guid CategoryId { get; private set; }
    public ProductStatus Status { get; private set; }

    // Reviews snapshot caching
    public float AverageRating { get; private set; }
    public int ReviewCount { get; private set; }

    // Soft Deletable
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUTC { get; private set; }

    private readonly List<ProductVariant> _variants = new();
    public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();

    private readonly List<ProductImage> _images = new();
    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

    private Product() { } // EF Core

    private Product(Guid id, string name, string description, decimal price, Guid categoryId) : base(id)
    {
        Name = name;
        Description = description;
        BasePrice = price;
        DiscountAmount = 0m;
        DiscountPercentage = 0m;
        CategoryId = categoryId;
        Status = ProductStatus.Draft;
        AverageRating = 0f;
        ReviewCount = 0;
        IsDeleted = false;
    }

    public static Result<Product> Create(string name, string description, decimal price, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Product>.Invalid(ProductErrors.NameIsRequired.ToValidationError());

        if (price <= 0)
            return Result<Product>.Invalid(ProductErrors.InvalidPrice.ToValidationError());

        if (categoryId == Guid.Empty)
            return Result<Product>.Invalid(ProductErrors.CategoryIdIsRequired.ToValidationError());

        var product = new Product(Guid.CreateVersion7(), name, description, price, categoryId);

        product.RaiseDomainEvent(new ProductCreatedEvent(
            product.Id,
            product.Name,
            product.CategoryId,
            product.BasePrice));

        return Result<Product>.Success(product);
    }

    public Result UpdateDetails(string name, string description, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Invalid(ProductErrors.NameIsRequired.ToValidationError());

        if (categoryId == Guid.Empty)
            return Result.Invalid(ProductErrors.CategoryIdIsRequired.ToValidationError());

        Name = name;
        Description = description;
        CategoryId = categoryId;

        return Result.Success();
    }

    public Result UpdateBasePrice(decimal price)
    {
        if (price <= 0)
            return Result.Invalid(ProductErrors.InvalidPrice.ToValidationError());

        BasePrice = price;

        // Automatically recalculate discount amount if a percentage is active
        if (DiscountPercentage > 0)
        {
            DiscountAmount = (price * (DiscountPercentage / 100m));
        }

        return Result.Success();
    }

    public Result ApplyDiscount(decimal discountPercentage)
    {
        if (discountPercentage <= 0 || discountPercentage > 100)
            return Result.Invalid(ProductErrors.InvalidDiscountPercentage.ToValidationError());

        DiscountPercentage = discountPercentage;
        DiscountAmount = (BasePrice * (discountPercentage / 100m));

        return Result.Success();
    }

    public Result RemoveDiscount()
    {
        if (DiscountPercentage == 0m)
            return Result.Invalid(ProductErrors.NoDiscountToRemove.ToValidationError());

        DiscountPercentage = 0m;
        DiscountAmount = 0m;

        return Result.Success();
    }

    public Result AddImage(string url, bool isPrimary)
    {
        var result = ProductImage.Create(url, isPrimary);
        if (!result.IsSuccess)
            return Result.Error(); // Error details inside validation

        if (isPrimary && _images.Any())
        {
            foreach (var img in _images)
                img.RemoveAsPrimary();
        }

        _images.Add(result.Value);
        return Result.Success();
    }

    public Result AddVariant(string sku, string color, ProductVariantSize size, int initialStock)
    {
        var result = ProductVariant.Create(sku, color, size, initialStock);
        if (!result.IsSuccess)
            return Result.Invalid(result.ValidationErrors);

        if (_variants.Any(v => v.SKU == sku))
            return Result.Conflict(ProductErrors.DuplicateSKU.Description);

        _variants.Add(result.Value);
        return Result.Success();
    }

    public Result Publish()
    {
        if (Status == ProductStatus.Published)
            return Result.Success();

        if (!_variants.Any())
            return Result.Invalid(ProductErrors.ProductHasNoVariants.ToValidationError());

        Status = ProductStatus.Published;
        RaiseDomainEvent(new ProductPublishedEvent(Id));
        return Result.Success();
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAtUTC = DateTime.UtcNow;
        Status = ProductStatus.Archived;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAtUTC = null;
        Status = ProductStatus.Draft;
    }

    public Result UpdateRating(int newStars)
    {
        if (newStars < 1 || newStars > 5)
            return Result.Invalid(ProductErrors.InvalidStarRating.ToValidationError());

        float totalStars = AverageRating * ReviewCount;
        totalStars += newStars;
        ReviewCount++;
        AverageRating = totalStars / ReviewCount;

        return Result.Success();
    }

    // Variant interaction via Root
    public Result AddStockToVariant(Guid variantId, int quantity)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId);
        if (variant == null)
            return Result.NotFound(ProductErrors.VariantNotFound.Description);

        return variant.Restock(quantity);
    }

    public Result RemoveStockFromVariant(Guid variantId, int quantity)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId);
        if (variant == null)
            return Result.NotFound(ProductErrors.VariantNotFound.Description);

        return variant.CommitStock(quantity);
    }
}
