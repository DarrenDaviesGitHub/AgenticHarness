using Ardent.Project.Application.Common.Exceptions;
using Ardent.Project.Application.Common.Interfaces;
using Ardent.Project.Domain.Products;
using MediatR;

namespace Ardent.Project.Application.Products.UpdateProduct;

/// <summary>Handles <see cref="UpdateProductCommand"/>.</summary>
public sealed class UpdateProductCommandHandler(
    IProductRepository products,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IRequestHandler<UpdateProductCommand>
{
    /// <inheritdoc />
    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        Product product = await products.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        product.Update(request.Name, request.Description, request.Sku, request.Price, timeProvider.GetUtcNow());

        if (await products.SkuExistsAsync(product.Sku, product.Id, cancellationToken))
        {
            throw new ConflictException($"A product with SKU '{product.Sku}' already exists.");
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
