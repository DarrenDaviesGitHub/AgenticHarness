using MediatR;

namespace Ardent.Project.Application.Products.UpdateProduct;

/// <summary>Replaces the editable fields of an existing product.</summary>
public sealed record UpdateProductCommand(Guid Id, string Name, string? Description, string Sku, decimal Price)
    : IRequest;
