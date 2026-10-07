using Ardent.Project.Application.Common.Exceptions;
using Ardent.Project.Application.Common.Interfaces;
using Ardent.Project.Application.Products.DeleteProduct;
using Ardent.Project.Domain.Products;

namespace Ardent.Project.Application.UnitTests.Products.DeleteProduct;

public sealed class DeleteProductCommandHandlerTests
{
    private readonly IProductRepository _products = Substitute.For<IProductRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly DeleteProductCommandHandler _handler;

    public DeleteProductCommandHandlerTests()
    {
        _handler = new DeleteProductCommandHandler(_products, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ExistingProduct_RemovesAndSaves()
    {
        Product product = TestData.Product();
        _products.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        await _handler.Handle(new DeleteProductCommand(product.Id), TestContext.Current.CancellationToken);

        _products.Received(1).Remove(product);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MissingProduct_ThrowsNotFoundException()
    {
        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new DeleteProductCommand(Guid.CreateVersion7()), TestContext.Current.CancellationToken));

        _products.DidNotReceive().Remove(Arg.Any<Product>());
    }
}
