using FluentValidation;

namespace Ardent.Project.Application.Products.ListProducts;

/// <summary>Validates paging input for <see cref="ListProductsQuery"/>.</summary>
public sealed class ListProductsQueryValidator : AbstractValidator<ListProductsQuery>
{
    /// <summary>Defines the rules.</summary>
    public ListProductsQueryValidator()
    {
        RuleFor(q => q.Page).InclusiveBetween(1, ListProductsQuery.MaxPage);
        RuleFor(q => q.PageSize).InclusiveBetween(1, ListProductsQuery.MaxPageSize);
    }
}
