using Ardent.Project.Application.Common.Exceptions;
using Ardent.Project.Application.Common.Interfaces;
using Ardent.Project.Application.Products;
using Ardent.Project.Application.Products.GetProductById;
using Ardent.Project.Domain.Products;

namespace Ardent.Project.Application.UnitTests.Products.GetProductById;

public sealed class GetProductByIdQueryHandlerTests
{
    private readonly IProductRepository _products = Substitute.For<IProductRepository>();
    private readonly GetProductByIdQueryHandler _handler;

    public GetProductByIdQueryHandlerTests()
    {
        _handler = new GetProductByIdQueryHandler(_products);
    }

    [Fact]
    public async Task Handle_ExistingProduct_ReturnsMappedResponse()
    {
        Product product = TestData.Product(sku: "KB-777", price: 42m);
        _products.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        ProductResponse response = await _handler.Handle(new GetProductByIdQuery(product.Id), TestContext.Current.CancellationToken);

        response.Id.ShouldBe(product.Id);
        response.Sku.ShouldBe("KB-777");
        response.Price.ShouldBe(42m);
    }

    [Fact]
    public async Task Handle_MissingProduct_ThrowsNotFoundException()
    {
        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new GetProductByIdQuery(Guid.CreateVersion7()), TestContext.Current.CancellationToken));
    }
}
