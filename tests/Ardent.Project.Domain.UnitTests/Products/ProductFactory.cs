using Ardent.Project.Domain.Products;

namespace Ardent.Project.Domain.UnitTests.Products;

internal static class ProductFactory
{
    public static readonly DateTimeOffset CreatedAt = new(2026, 1, 15, 9, 30, 0, TimeSpan.Zero);

    public static Product Create(
        string name = "Mechanical keyboard",
        string? description = "Tenkeyless",
        string sku = "KB-001",
        decimal price = 129.99m) =>
        Product.Create(name, description, sku, price, CreatedAt);
}
