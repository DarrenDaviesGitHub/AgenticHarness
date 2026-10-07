using Ardent.Project.Application.Products.CreateProduct;
using Ardent.Project.Domain.Products;
using FluentValidation.TestHelper;

namespace Ardent.Project.Application.UnitTests.Products.CreateProduct;

public sealed class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        TestValidationResult<CreateProductCommand> result = _validator.TestValidate(TestData.CreateCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyName_HasNameError()
    {
        TestValidationResult<CreateProductCommand> result = _validator.TestValidate(TestData.CreateCommand(name: ""));

        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Validate_NameLongerThanMax_HasNameError()
    {
        CreateProductCommand command = TestData.CreateCommand(name: new string('a', Product.NameMaxLength + 1));

        TestValidationResult<CreateProductCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Validate_DescriptionLongerThanMax_HasDescriptionError()
    {
        CreateProductCommand command = TestData.CreateCommand(description: new string('a', Product.DescriptionMaxLength + 1));

        TestValidationResult<CreateProductCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData("has space")]
    [InlineData("bad!chars")]
    public void Validate_InvalidSku_HasSkuError(string sku)
    {
        TestValidationResult<CreateProductCommand> result = _validator.TestValidate(TestData.CreateCommand(sku: sku));

        result.ShouldHaveValidationErrorFor(c => c.Sku);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_NonPositivePrice_HasPriceError(decimal price)
    {
        TestValidationResult<CreateProductCommand> result = _validator.TestValidate(TestData.CreateCommand(price: price));

        result.ShouldHaveValidationErrorFor(c => c.Price);
    }
}
