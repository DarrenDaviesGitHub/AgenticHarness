using MediatR;

namespace Ardent.Project.Application.Products.GetProductById;

/// <summary>Returns one product.</summary>
public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductResponse>;
