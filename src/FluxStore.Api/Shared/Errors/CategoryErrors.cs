using FluxStore.Api.Domain;

namespace FluxStore.Api.Shared.Errors;

public static class CategoryErrors
{
    public static readonly Error NameIsRequired = new("Category.NameRequired", "Category name is required.", ErrorType.Validation);
    public static readonly Error SlugIsRequired = new("Category.SlugRequired", "Category slug is required.", ErrorType.Validation);
    public static readonly Error CircularDependency = new("Category.CircularDependency", "A category cannot be its own parent.", ErrorType.Conflict);
}
