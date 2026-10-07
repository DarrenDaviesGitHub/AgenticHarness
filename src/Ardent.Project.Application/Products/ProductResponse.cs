using Ardent.Project.Domain.Products;

namespace Ardent.Project.Application.Products;

/// <summary>A product as returned by the API.</summary>
/// <param name="Id">Unique identifier.</param>
/// <param name="Name">Display name.</param>
/// <param name="Description">Optional long description.</param>
/// <param name="Sku">Stock keeping unit (upper-case, unique).</param>
/// <param name="Price">Unit price.</param>
/// <param name="CreatedAt">When the product was created (UTC).</param>
/// <param name="UpdatedAt">When the product was last updated (UTC), if ever.</param>
public sealed record ProductResponse(
    Guid Id,
    string Name,
    string? Description,
    string Sku,
    decimal Price,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt)
{
    /// <summary>Maps a domain product to its response shape.</summary>
    public static ProductResponse FromProduct(Product product) =>
        new(product.Id, product.Name, product.Description, product.Sku, product.Price, product.CreatedAt, product.UpdatedAt);
}
