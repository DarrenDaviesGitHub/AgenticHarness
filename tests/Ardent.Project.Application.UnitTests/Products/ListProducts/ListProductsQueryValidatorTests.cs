using Ardent.Project.Application.Products.ListProducts;
using FluentValidation.TestHelper;

namespace Ardent.Project.Application.UnitTests.Products.ListProducts;

public sealed class ListProductsQueryValidatorTests
{
    private readonly ListProductsQueryValidator _validator = new();

    [Theory]
    [InlineData(1, 1)]
    [InlineData(5, ListProductsQuery.MaxPageSize)]
    public void Validate_PagingInRange_HasNoErrors(int page, int pageSize)
    {
        TestValidationResult<ListProductsQuery> result = _validator.TestValidate(new ListProductsQuery(page, pageSize));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(ListProductsQuery.MaxPage + 1)]
    public void Validate_PageOutOfRange_HasPageError(int page)
    {
        TestValidationResult<ListProductsQuery> result = _validator.TestValidate(new ListProductsQuery(page, 20));

        result.ShouldHaveValidationErrorFor(q => q.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(ListProductsQuery.MaxPageSize + 1)]
    public void Validate_PageSizeOutOfRange_HasPageSizeError(int pageSize)
    {
        TestValidationResult<ListProductsQuery> result = _validator.TestValidate(new ListProductsQuery(1, pageSize));

        result.ShouldHaveValidationErrorFor(q => q.PageSize);
    }
}
