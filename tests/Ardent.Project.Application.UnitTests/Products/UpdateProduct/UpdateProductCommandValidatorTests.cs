using Ardent.Project.Application.Products.UpdateProduct;
using FluentValidation.TestHelper;

namespace Ardent.Project.Application.UnitTests.Products.UpdateProduct;

public sealed class UpdateProductCommandValidatorTests
{
    private readonly UpdateProductCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        TestValidationResult<UpdateProductCommand> result = _validator.TestValidate(TestData.UpdateCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyName_HasNameError()
    {
        TestValidationResult<UpdateProductCommand> result = _validator.TestValidate(TestData.UpdateCommand(name: ""));

        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Validate_SkuWithSpaces_HasSkuError()
    {
        TestValidationResult<UpdateProductCommand> result = _validator.TestValidate(TestData.UpdateCommand(sku: "has space"));

        result.ShouldHaveValidationErrorFor(c => c.Sku);
    }

    [Fact]
    public void Validate_ZeroPrice_HasPriceError()
    {
        TestValidationResult<UpdateProductCommand> result = _validator.TestValidate(TestData.UpdateCommand(price: 0));

        result.ShouldHaveValidationErrorFor(c => c.Price);
    }
}
