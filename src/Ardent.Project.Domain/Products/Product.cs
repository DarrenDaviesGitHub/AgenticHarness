using System.Text.RegularExpressions;
using Ardent.Project.Domain.Common;

namespace Ardent.Project.Domain.Products;

/// <summary>
/// A product in the catalogue. Created only through <see cref="Create"/> and changed only through <see cref="Update"/>,
/// so every instance satisfies its invariants.
/// </summary>
public sealed class Product
{
    /// <summary>Maximum length of <see cref="Name"/>.</summary>
    public const int NameMaxLength = 200;

    /// <summary>Maximum length of <see cref="Description"/>.</summary>
    public const int DescriptionMaxLength = 2000;

    /// <summary>Maximum length of <see cref="Sku"/>.</summary>
    public const int SkuMaxLength = 64;

    /// <summary>Allowed SKU characters: letters, digits and hyphens.</summary>
    public const string SkuPattern = "^[A-Za-z0-9-]+$";

    // Used by EF Core to materialise rows; it writes the backing fields directly, bypassing the guards.
    private Product()
    {
    }

    /// <summary>Unique identifier (UUID v7, so it sorts by creation time).</summary>
    public Guid Id { get; private set; }

    /// <summary>Display name.</summary>
    public string Name
    {
        get;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new DomainException("Product name is required.");
            }

            string trimmed = value.Trim();
            if (trimmed.Length > NameMaxLength)
            {
                throw new DomainException($"Product name must be at most {NameMaxLength} characters.");
            }

            field = trimmed;
        }
    } = string.Empty;

    /// <summary>Optional long description.</summary>
    public string? Description
    {
        get;
        private set
        {
            string? trimmed = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
            if (trimmed?.Length > DescriptionMaxLength)
            {
                throw new DomainException($"Product description must be at most {DescriptionMaxLength} characters.");
            }

            field = trimmed;
        }
    }

    /// <summary>Stock keeping unit, stored upper-case and unique across all products.</summary>
    public string Sku
    {
        get;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new DomainException("Product SKU is required.");
            }

            string normalised = value.Trim().ToUpperInvariant();
            if (normalised.Length > SkuMaxLength)
            {
                throw new DomainException($"Product SKU must be at most {SkuMaxLength} characters.");
            }

            if (!Regex.IsMatch(normalised, SkuPattern))
            {
                throw new DomainException("Product SKU may contain only letters, digits and hyphens.");
            }

            field = normalised;
        }
    } = string.Empty;

    /// <summary>Unit price; always greater than zero.</summary>
    public decimal Price
    {
        get;
        private set
        {
            if (value <= 0)
            {
                throw new DomainException("Product price must be greater than zero.");
            }

            field = value;
        }
    }

    /// <summary>When the product was created (UTC).</summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>When the product was last updated (UTC), or null if never updated.</summary>
    public DateTimeOffset? UpdatedAt { get; private set; }

    /// <summary>Creates a valid product or throws <see cref="DomainException"/>.</summary>
    public static Product Create(string name, string? description, string sku, decimal price, DateTimeOffset createdAt) =>
        new()
        {
            Id = Guid.CreateVersion7(createdAt),
            Name = name,
            Description = description,
            Sku = sku,
            Price = price,
            CreatedAt = createdAt,
        };

    /// <summary>Replaces the editable fields or throws <see cref="DomainException"/>, leaving the product unchanged.</summary>
    public void Update(string name, string? description, string sku, decimal price, DateTimeOffset updatedAt)
    {
        // Validate everything against a candidate first so a failure never leaves the entity half-updated.
        Product candidate = Create(name, description, sku, price, CreatedAt);

        Name = candidate.Name;
        Description = candidate.Description;
        Sku = candidate.Sku;
        Price = candidate.Price;
        UpdatedAt = updatedAt;
    }
}
