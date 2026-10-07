using Ardent.Project.Application.Common.Exceptions;
using Ardent.Project.Application.Common.Interfaces;
using Ardent.Project.Domain.Products;
using MediatR;

namespace Ardent.Project.Application.Products.CreateProduct;

/// <summary>Handles <see cref="CreateProductCommand"/>.</summary>
public sealed class CreateProductCommandHandler(
    IProductRepository products,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IRequestHandler<CreateProductCommand, ProductResponse>
{
    /// <inheritdoc />
    public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        Product product = Product.Create(request.Name, request.Description, request.Sku, request.Price, timeProvider.GetUtcNow());

        if (await products.SkuExistsAsync(product.Sku, excludingProductId: null, cancellationToken))
        {
            throw new ConflictException($"A product with SKU '{product.Sku}' already exists.");
        }

        products.Add(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ProductResponse.FromProduct(product);
    }
}
