using Ardent.Project.Application.Products.UpdateProduct;

namespace Ardent.Project.Api.Contracts;

/// <summary>Body of <c>PUT /api/products/{id}</c>. Replaces every editable field.</summary>
/// <param name="Name">Display name (required, max 200 characters).</param>
/// <param name="Description">Optional long description (max 2000 characters).</param>
/// <param name="Sku">Stock keeping unit: letters, digits and hyphens; unique; stored upper-case.</param>
/// <param name="Price">Unit price; must be greater than zero.</param>
public sealed record UpdateProductRequest(string Name, string? Description, string Sku, decimal Price)
{
    /// <summary>Maps the HTTP contract to the application command for the product in the route.</summary>
    public UpdateProductCommand ToCommand(Guid id) => new(id, Name, Description, Sku, Price);
}
