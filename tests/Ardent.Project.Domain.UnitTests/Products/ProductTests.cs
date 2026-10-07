using Ardent.Project.Domain.Common;
using Ardent.Project.Domain.Products;

namespace Ardent.Project.Domain.UnitTests.Products;

public sealed class ProductTests
{
    [Fact]
    public void Create_ValidInput_SetsAllProperties()
    {
        Product product = ProductFactory.Create(name: "Keyboard", description: "Compact", sku: "KB-1", price: 10.5m);

        product.Name.ShouldBe("Keyboard");
        product.Description.ShouldBe("Compact");
        product.Sku.ShouldBe("KB-1");
        product.Price.ShouldBe(10.5m);
        product.CreatedAt.ShouldBe(ProductFactory.CreatedAt);
        product.UpdatedAt.ShouldBeNull();
    }

    [Fact]
    public void Create_ValidInput_GeneratesVersion7Id()
    {
        Product product = ProductFactory.Create();

        product.Id.Version.ShouldBe(7);
    }

    [Fact]
    public void Create_LowerCaseSkuWithPadding_StoresTrimmedUpperCaseSku()
    {
        Product product = ProductFactory.Create(sku: "  kb-001 ");

        product.Sku.ShouldBe("KB-001");
    }

    [Fact]
    public void Create_NameWithPadding_StoresTrimmedName()
    {
        Product product = ProductFactory.Create(name: "  Keyboard  ");

        product.Name.ShouldBe("Keyboard");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WhitespaceDescription_StoresNull(string description)
    {
        Product product = ProductFactory.Create(description: description);

        product.Description.ShouldBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_BlankName_ThrowsDomainException(string name)
    {
        Should.Throw<DomainException>(() => ProductFactory.Create(name: name))
            .Message.ShouldContain("name is required");
    }

    [Fact]
    public void Create_NameLongerThanMax_ThrowsDomainException()
    {
        string name = new('a', Product.NameMaxLength + 1);

        Should.Throw<DomainException>(() => ProductFactory.Create(name: name));
    }

    [Fact]
    public void Create_DescriptionLongerThanMax_ThrowsDomainException()
    {
        string description = new('a', Product.DescriptionMaxLength + 1);

        Should.Throw<DomainException>(() => ProductFactory.Create(description: description));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_BlankSku_ThrowsDomainException(string sku)
    {
        Should.Throw<DomainException>(() => ProductFactory.Create(sku: sku))
            .Message.ShouldContain("SKU is required");
    }

    [Theory]
    [InlineData("has space")]
    [InlineData("bad!chars")]
    public void Create_SkuWithInvalidCharacters_ThrowsDomainException(string sku)
    {
        Should.Throw<DomainException>(() => ProductFactory.Create(sku: sku))
            .Message.ShouldContain("letters, digits and hyphens");
    }

    [Fact]
    public void Create_SkuLongerThanMax_ThrowsDomainException()
    {
        string sku = new('A', Product.SkuMaxLength + 1);

        Should.Throw<DomainException>(() => ProductFactory.Create(sku: sku));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    public void Create_NonPositivePrice_ThrowsDomainException(decimal price)
    {
        Should.Throw<DomainException>(() => ProductFactory.Create(price: price))
            .Message.ShouldContain("greater than zero");
    }

    [Fact]
    public void Update_ValidInput_ReplacesFieldsAndSetsUpdatedAt()
    {
        Product product = ProductFactory.Create();
        DateTimeOffset updatedAt = ProductFactory.CreatedAt.AddDays(1);

        product.Update("New name", null, "new-sku", 5m, updatedAt);

        product.Name.ShouldBe("New name");
        product.Description.ShouldBeNull();
        product.Sku.ShouldBe("NEW-SKU");
        product.Price.ShouldBe(5m);
        product.UpdatedAt.ShouldBe(updatedAt);
    }

    [Fact]
    public void Update_ValidInput_KeepsIdAndCreatedAt()
    {
        Product product = ProductFactory.Create();
        Guid originalId = product.Id;

        product.Update("New name", null, "NEW-SKU", 5m, ProductFactory.CreatedAt.AddDays(1));

        product.Id.ShouldBe(originalId);
        product.CreatedAt.ShouldBe(ProductFactory.CreatedAt);
    }

    [Fact]
    public void Update_InvalidPrice_ThrowsAndLeavesProductUnchanged()
    {
        Product product = ProductFactory.Create(name: "Original", price: 10m);

        Should.Throw<DomainException>(() => product.Update("Changed", null, "KB-001", 0m, ProductFactory.CreatedAt));

        product.Name.ShouldBe("Original");
        product.Price.ShouldBe(10m);
        product.UpdatedAt.ShouldBeNull();
    }
}
