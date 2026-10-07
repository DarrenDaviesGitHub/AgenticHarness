using Ardent.Project.Application.Common.Exceptions;
using Ardent.Project.Application.Common.Interfaces;
using Ardent.Project.Application.Products.UpdateProduct;
using Ardent.Project.Domain.Products;

namespace Ardent.Project.Application.UnitTests.Products.UpdateProduct;

public sealed class UpdateProductCommandHandlerTests
{
    private readonly IProductRepository _products = Substitute.For<IProductRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdateProductCommandHandler _handler;

    public UpdateProductCommandHandlerTests()
    {
        _handler = new UpdateProductCommandHandler(_products, _unitOfWork, TestData.Clock());
    }

    [Fact]
    public async Task Handle_ExistingProduct_UpdatesFieldsAndSaves()
    {
        Product product = TestData.Product();
        _products.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        await _handler.Handle(TestData.UpdateCommand(product.Id, name: "Renamed", sku: "kb-new"), TestContext.Current.CancellationToken);

        product.Name.ShouldBe("Renamed");
        product.Sku.ShouldBe("KB-NEW");
        product.UpdatedAt.ShouldBe(TestData.Now);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MissingProduct_ThrowsNotFoundException()
    {
        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(TestData.UpdateCommand(), TestContext.Current.CancellationToken));

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SkuUsedByAnotherProduct_ThrowsConflictExceptionWithoutSaving()
    {
        Product product = TestData.Product();
        _products.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _products.SkuExistsAsync("TAKEN-1", product.Id, Arg.Any<CancellationToken>()).Returns(true);

        await Should.ThrowAsync<ConflictException>(() =>
            _handler.Handle(TestData.UpdateCommand(product.Id, sku: "TAKEN-1"), TestContext.Current.CancellationToken));

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
