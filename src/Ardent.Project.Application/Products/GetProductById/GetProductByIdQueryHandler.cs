using Ardent.Project.Application.Common.Exceptions;
using Ardent.Project.Application.Common.Interfaces;
using Ardent.Project.Domain.Products;
using MediatR;

namespace Ardent.Project.Application.Products.GetProductById;

/// <summary>Handles <see cref="GetProductByIdQuery"/>.</summary>
public sealed class GetProductByIdQueryHandler(IProductRepository products)
    : IRequestHandler<GetProductByIdQuery, ProductResponse>
{
    /// <inheritdoc />
    public async Task<ProductResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        Product product = await products.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        return ProductResponse.FromProduct(product);
    }
}
