using MediatR;

namespace Ardent.Project.Application.Products.CreateProduct;

/// <summary>Creates a product with a unique SKU.</summary>
public sealed record CreateProductCommand(string Name, string? Description, string Sku, decimal Price)
    : IRequest<ProductResponse>;
