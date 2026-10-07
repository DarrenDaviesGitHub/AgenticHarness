using Ardent.Project.Domain.Products;

namespace Ardent.Project.Application.Common.Interfaces;

/// <summary>Persistence operations for <see cref="Product"/>. Changes are saved through <see cref="IUnitOfWork"/>.</summary>
public interface IProductRepository
{
    /// <summary>Returns the product, tracked for changes, or null if it does not exist.</summary>
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Returns one page of products ordered by name, read-only.</summary>
    Task<IReadOnlyList<Product>> ListAsync(int skip, int take, CancellationToken cancellationToken);

    /// <summary>Returns the total number of products.</summary>
    Task<int> CountAsync(CancellationToken cancellationToken);

    /// <summary>Returns true if another product already uses the SKU.</summary>
    Task<bool> SkuExistsAsync(string sku, Guid? excludingProductId, CancellationToken cancellationToken);

    /// <summary>Stages a new product for insertion.</summary>
    void Add(Product product);

    /// <summary>Stages a product for deletion.</summary>
    void Remove(Product product);
}
