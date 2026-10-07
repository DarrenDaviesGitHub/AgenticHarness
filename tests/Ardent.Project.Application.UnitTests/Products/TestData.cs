using Ardent.Project.Application.Products.CreateProduct;
using Ardent.Project.Application.Products.UpdateProduct;
using Ardent.Project.Domain.Products;

namespace Ardent.Project.Application.UnitTests.Products;

internal static class TestData
{
    public static readonly DateTimeOffset Now = new(2026, 3, 1, 12, 0, 0, TimeSpan.Zero);

    public static Product Product(string sku = "KB-001", decimal price = 129.99m) =>
        Domain.Products.Product.Create("Mechanical keyboard", "Tenkeyless", sku, price, Now.AddDays(-7));

    public static CreateProductCommand CreateCommand(
        string name = "Mechanical keyboard",
        string? description = "Tenkeyless",
        string sku = "KB-001",
        decimal price = 129.99m) =>
        new(name, description, sku, price);

    public static UpdateProductCommand UpdateCommand(
        Guid? id = null,
        string name = "Mechanical keyboard v2",
        string sku = "KB-002",
        decimal price = 139.99m) =>
        new(id ?? Guid.CreateVersion7(), name, "PBT keycaps", sku, price);

    public static TimeProvider Clock()
    {
        TimeProvider clock = Substitute.For<TimeProvider>();
        clock.GetUtcNow().Returns(Now);
        return clock;
    }
}
