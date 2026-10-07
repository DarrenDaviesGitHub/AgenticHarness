using Ardent.Project.Application.Common.Interfaces;
using Ardent.Project.Application.Common.Models;
using Ardent.Project.Application.Products;
using Ardent.Project.Application.Products.ListProducts;
using Ardent.Project.Domain.Products;

namespace Ardent.Project.Application.UnitTests.Products.ListProducts;

public sealed class ListProductsQueryHandlerTests
{
    private readonly IProductRepository _products = Substitute.For<IProductRepository>();
    private readonly ListProductsQueryHandler _handler;

    public ListProductsQueryHandlerTests()
    {
        _handler = new ListProductsQueryHandler(_products);
    }

    [Fact]
    public async Task Handle_SecondPage_SkipsFirstPageAndReturnsPagingMetadata()
    {
        Product product = TestData.Product();
        _products.ListAsync(10, 10, Arg.Any<CancellationToken>()).Returns([product]);
        _products.CountAsync(Arg.Any<CancellationToken>()).Returns(11);

        PagedResponse<ProductResponse> response = await _handler.Handle(new ListProductsQuery(2, 10), TestContext.Current.CancellationToken);

        response.Items.ShouldHaveSingleItem().Id.ShouldBe(product.Id);
        response.Page.ShouldBe(2);
        response.PageSize.ShouldBe(10);
        response.TotalCount.ShouldBe(11);
        response.TotalPages.ShouldBe(2);
    }

    [Fact]
    public async Task Handle_NoProducts_ReturnsEmptyPage()
    {
        _products.ListAsync(0, 20, Arg.Any<CancellationToken>()).Returns([]);

        PagedResponse<ProductResponse> response = await _handler.Handle(new ListProductsQuery(1, 20), TestContext.Current.CancellationToken);

        response.Items.ShouldBeEmpty();
        response.TotalPages.ShouldBe(0);
    }
}
