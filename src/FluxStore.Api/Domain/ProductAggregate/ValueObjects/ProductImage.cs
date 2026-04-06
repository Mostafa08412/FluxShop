using Ardalis.Result;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Extensions;

namespace FluxStore.Api.Domain.ProductAggregate.ValueObjects;

public sealed class ProductImage : ValueObject
{
    public string Url { get; }
    public bool IsPrimary { get; private set; }

    private ProductImage(string url, bool isPrimary)
    {
        Url = url;
        IsPrimary = isPrimary;
    }

    public static Result<ProductImage> Create(string url, bool isPrimary = false)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return Result<ProductImage>.Invalid(ProductErrors.ImageUrlIsRequired.ToValidationError());
        }

        return Result<ProductImage>.Success(new ProductImage(url, isPrimary));
    }

    public void SetAsPrimary()
    {
        IsPrimary = true;
    }

    public void RemoveAsPrimary()
    {
        IsPrimary = false;
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Url;
    }
}
