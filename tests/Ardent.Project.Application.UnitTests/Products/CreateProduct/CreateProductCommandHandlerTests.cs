using Ardent.Project.Application.Common.Exceptions;
using Ardent.Project.Application.Common.Interfaces;
using Ardent.Project.Application.Products;
using Ardent.Project.Application.Products.CreateProduct;
using Ardent.Project.Domain.Products;

namespace Ardent.Project.Application.UnitTests.Products.CreateProduct;

public sealed class CreateProductCommandHandlerTests
{
    private readonly IProductRepository _products = Substitute.For<IProductRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _handler = new CreateProductCommandHandler(_products, _unitOfWork, TestData.Clock());
    }

    [Fact]
    public async Task Handle_UniqueSku_AddsProductAndSaves()
    {
        CreateProductCommand command = TestData.CreateCommand(sku: "kb-001");

        ProductResponse response = await _handler.Handle(command, TestContext.Current.CancellationToken);

        _products.Received(1).Add(Arg.Is<Product>(p => p.Sku == "KB-001"));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        response.Sku.ShouldBe("KB-001");
    }

    [Fact]
    public async Task Handle_UniqueSku_StampsCreatedAtFromTimeProvider()
    {
        ProductResponse response = await _handler.Handle(TestData.CreateCommand(), TestContext.Current.CancellationToken);

        response.CreatedAt.ShouldBe(TestData.Now);
    }

    [Fact]
    public async Task Handle_DuplicateSku_ThrowsConflictExceptionWithoutSaving()
    {
        _products.SkuExistsAsync("KB-001", null, Arg.Any<CancellationToken>()).Returns(true);

        await Should.ThrowAsync<ConflictException>(() =>
            _handler.Handle(TestData.CreateCommand(sku: "KB-001"), TestContext.Current.CancellationToken));

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
