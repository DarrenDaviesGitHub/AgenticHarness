using Ardent.Project.Application.Products.CreateProduct;

namespace Ardent.Project.Api.Contracts;

/// <summary>Body of <c>POST /api/products</c>.</summary>
/// <param name="Name">Display name (required, max 200 characters).</param>
/// <param name="Description">Optional long description (max 2000 characters).</param>
/// <param name="Sku">Stock keeping unit: letters, digits and hyphens; unique; stored upper-case.</param>
/// <param name="Price">Unit price; must be greater than zero.</param>
public sealed record CreateProductRequest(string Name, string? Description, string Sku, decimal Price)
{
    /// <summary>Maps the HTTP contract to the application command.</summary>
    public CreateProductCommand ToCommand() => new(Name, Description, Sku, Price);
}
