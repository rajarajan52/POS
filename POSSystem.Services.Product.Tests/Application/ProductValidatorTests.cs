using POSSystem.Domain.Domain.Models;
using ProductModel = POSSystem.Domain.Domain.Models.Product;
using ProductValidator = POSSystem.Services.Product.Application.Validators.ProductValidator;
using FluentAssertions;
using Xunit;

namespace POSSystem.Services.Product.Tests.Application;

/// <summary>
/// Unit tests for ProductValidator.
/// Tests business rule validation following SOLID principles.
/// </summary>
public class ProductValidatorTests
{
    private readonly ProductValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidProduct_ReturnsSuccessfully()
    {
        // Arrange
        var product = new ProductModel
        {
            Name = "Valid Product",
            Sku = "VALID-001",
            Price = 29.99m,
            CategoryId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _validator.ValidateAsync(product);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_WithMissingName_ReturnsFail()
    {
        // Arrange
        var product = new ProductModel
        {
            Name = string.Empty,
            Sku = "VALID-001",
            Price = 29.99m,
            CategoryId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _validator.ValidateAsync(product);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Validate_WithInvalidSku_ReturnsFail()
    {
        // Arrange
        var product = new ProductModel
        {
            Name = "Valid Product",
            Sku = "invalid-sku",  // lowercase not allowed
            Price = 29.99m,
            CategoryId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _validator.ValidateAsync(product);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Validate_WithNegativePrice_ReturnsFail()
    {
        // Arrange
        var product = new ProductModel
        {
            Name = "Valid Product",
            Sku = "VALID-001",
            Price = -10m,
            CategoryId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _validator.ValidateAsync(product);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }
}


