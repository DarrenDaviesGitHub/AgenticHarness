using Ardent.Project.Application.Common.Interfaces;
using Ardent.Project.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace Ardent.Project.Infrastructure.Persistence.Repositories;

/// <summary>EF Core implementation of <see cref="IProductRepository"/>.</summary>
public sealed class ProductRepository(ApplicationDbContext dbContext) : IProductRepository
{
    /// <inheritdoc />
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Product>> ListAsync(int skip, int take, CancellationToken cancellationToken) =>
        await dbContext.Products
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ThenBy(p => p.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public Task<int> CountAsync(CancellationToken cancellationToken) =>
        dbContext.Products.CountAsync(cancellationToken);

    /// <inheritdoc />
    public Task<bool> SkuExistsAsync(string sku, Guid? excludingProductId, CancellationToken cancellationToken) =>
        dbContext.Products.AnyAsync(p => p.Sku == sku && p.Id != excludingProductId, cancellationToken);

    /// <inheritdoc />
    public void Add(Product product) => dbContext.Products.Add(product);

    /// <inheritdoc />
    public void Remove(Product product) => dbContext.Products.Remove(product);
}
