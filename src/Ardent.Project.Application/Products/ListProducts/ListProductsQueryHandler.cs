using Ardent.Project.Application.Common.Interfaces;
using Ardent.Project.Application.Common.Models;
using Ardent.Project.Domain.Products;
using MediatR;

namespace Ardent.Project.Application.Products.ListProducts;

/// <summary>Handles <see cref="ListProductsQuery"/>.</summary>
public sealed class ListProductsQueryHandler(IProductRepository products)
    : IRequestHandler<ListProductsQuery, PagedResponse<ProductResponse>>
{
    /// <inheritdoc />
    public async Task<PagedResponse<ProductResponse>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        int skip = (request.Page - 1) * request.PageSize;
        IReadOnlyList<Product> page = await products.ListAsync(skip, request.PageSize, cancellationToken);
        int totalCount = await products.CountAsync(cancellationToken);

        return new PagedResponse<ProductResponse>(
            [.. page.Select(ProductResponse.FromProduct)],
            request.Page,
            request.PageSize,
            totalCount);
    }
}
