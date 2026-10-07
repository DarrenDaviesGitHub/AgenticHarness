using Ardent.Project.Domain.Products;
using FluentValidation;

namespace Ardent.Project.Application.Products.CreateProduct;

/// <summary>Validates <see cref="CreateProductCommand"/> input before it reaches the domain.</summary>
public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    /// <summary>Defines the rules.</summary>
    public CreateProductCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(Product.NameMaxLength);
        RuleFor(c => c.Description).MaximumLength(Product.DescriptionMaxLength);
        RuleFor(c => c.Sku).NotEmpty().MaximumLength(Product.SkuMaxLength)
            .Matches(Product.SkuPattern).WithMessage("SKU may contain only letters, digits and hyphens.");
        RuleFor(c => c.Price).GreaterThan(0);
    }
}
