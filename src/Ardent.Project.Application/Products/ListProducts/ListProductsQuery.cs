using Ardent.Project.Application.Common.Models;
using MediatR;

namespace Ardent.Project.Application.Products.ListProducts;

/// <summary>Returns one page of products ordered by name.</summary>
public sealed record ListProductsQuery(int Page, int PageSize) : IRequest<PagedResponse<ProductResponse>>
{
    /// <summary>Largest page a client may request.</summary>
    public const int MaxPageSize = 100;

    /// <summary>Largest page number; keeps the row offset within <see cref="int"/>.</summary>
    public const int MaxPage = int.MaxValue / MaxPageSize;
}
