using Ardalis.Result;
using FluxStore.Api.Shared.Abstractions;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Extensions;

namespace FluxStore.Api.Domain.CategoryAggregate;

public sealed class Category : Aggregate, ISoftDeletable
{
    public string Name { get; private set; }
    public string Slug { get; private set; }
    public Guid? ParentCategoryId { get; private set; }

    // Soft Deletable
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUTC { get; private set; }

    private Category() { } // EF Core

    private Category(Guid id, string name, string slug, Guid? parentCategoryId) : base(id)
    {
        Name = name;
        Slug = slug;
        ParentCategoryId = parentCategoryId;
        IsDeleted = false;
    }

    public static Result<Category> Create(string name, string slug, Guid? parentCategoryId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Category>.Invalid(CategoryErrors.NameIsRequired.ToValidationError());

        if (string.IsNullOrWhiteSpace(slug))
            return Result<Category>.Invalid(CategoryErrors.SlugIsRequired.ToValidationError());

        var category = new Category(Guid.CreateVersion7(), name, slug.ToLowerInvariant(), parentCategoryId);
        return Result<Category>.Success(category);
    }

    public Result UpdateDetails(string name, string slug)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Invalid(CategoryErrors.NameIsRequired.ToValidationError());

        if (string.IsNullOrWhiteSpace(slug))
            return Result.Invalid(CategoryErrors.SlugIsRequired.ToValidationError());

        Name = name;
        Slug = slug.ToLowerInvariant();
        return Result.Success();
    }

    public Result MoveToParent(Guid? parentCategoryId)
    {
        if (parentCategoryId.HasValue && parentCategoryId.Value == Id)
            return Result.Invalid(CategoryErrors.CircularDependency.ToValidationError());

        ParentCategoryId = parentCategoryId;
        return Result.Success();
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAtUTC = DateTime.UtcNow;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAtUTC = null;
    }
}
