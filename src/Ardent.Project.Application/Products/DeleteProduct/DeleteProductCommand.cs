using MediatR;

namespace Ardent.Project.Application.Products.DeleteProduct;

/// <summary>Deletes a product.</summary>
public sealed record DeleteProductCommand(Guid Id) : IRequest;
