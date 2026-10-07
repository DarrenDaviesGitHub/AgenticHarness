using Ardent.Project.Api.Contracts;
using Ardent.Project.Application.Common.Models;
using Ardent.Project.Application.Products;
using Ardent.Project.Application.Products.DeleteProduct;
using Ardent.Project.Application.Products.GetProductById;
using Ardent.Project.Application.Products.ListProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ardent.Project.Api.Controllers;

/// <summary>Create, read, update and delete products.</summary>
[ApiController]
[Route("api/products")]
public sealed class ProductsController(ISender sender) : ControllerBase
{
    /// <summary>Lists products ordered by name, one page at a time.</summary>
    /// <param name="page">1-based page number.</param>
    /// <param name="pageSize">Items per page (1-100).</param>
    /// <param name="cancellationToken">Aborts the request.</param>
    /// <response code="200">The requested page.</response>
    /// <response code="400">Paging parameters are out of range.</response>
    [HttpGet]
    [ProducesResponseType<PagedResponse<ProductResponse>>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public async Task<ActionResult<PagedResponse<ProductResponse>>> ListAsync(
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default) =>
        Ok(await sender.Send(new ListProductsQuery(page, pageSize), cancellationToken));

    /// <summary>Gets one product.</summary>
    /// <param name="id">Product identifier.</param>
    /// <param name="cancellationToken">Aborts the request.</param>
    /// <response code="200">The product.</response>
    /// <response code="404">No product has this identifier.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    public async Task<ActionResult<ProductResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetProductByIdQuery(id), cancellationToken));

    /// <summary>Creates a product.</summary>
    /// <param name="request">The new product.</param>
    /// <param name="cancellationToken">Aborts the request.</param>
    /// <response code="201">Created; the Location header points to the new product.</response>
    /// <response code="400">The request is invalid.</response>
    /// <response code="409">Another product already uses this SKU.</response>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, "application/problem+json")]
    public async Task<ActionResult<ProductResponse>> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        ProductResponse product = await sender.Send(request.ToCommand(), cancellationToken);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = product.Id }, product);
    }

    /// <summary>Replaces every editable field of a product.</summary>
    /// <param name="id">Product identifier.</param>
    /// <param name="request">The new values.</param>
    /// <param name="cancellationToken">Aborts the request.</param>
    /// <response code="204">Updated.</response>
    /// <response code="400">The request is invalid.</response>
    /// <response code="404">No product has this identifier.</response>
    /// <response code="409">Another product already uses this SKU.</response>
    [HttpPut("{id:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, "application/problem+json")]
    public async Task<IActionResult> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(request.ToCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>Deletes a product.</summary>
    /// <param name="id">Product identifier.</param>
    /// <param name="cancellationToken">Aborts the request.</param>
    /// <response code="204">Deleted.</response>
    /// <response code="404">No product has this identifier.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteProductCommand(id), cancellationToken);
        return NoContent();
    }
}
