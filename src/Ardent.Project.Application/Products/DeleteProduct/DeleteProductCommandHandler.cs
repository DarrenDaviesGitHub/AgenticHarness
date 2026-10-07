using Ardent.Project.Application.Common.Exceptions;
using Ardent.Project.Application.Common.Interfaces;
using Ardent.Project.Domain.Products;
using MediatR;

namespace Ardent.Project.Application.Products.DeleteProduct;

/// <summary>Handles <see cref="DeleteProductCommand"/>.</summary>
public sealed class DeleteProductCommandHandler(IProductRepository products, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteProductCommand>
{
    /// <inheritdoc />
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        Product product = await products.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        products.Remove(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
