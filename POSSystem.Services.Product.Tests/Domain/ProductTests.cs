using POSSystem.Domain.Domain.Models;
using ProductModel = POSSystem.Domain.Domain.Models.Product;
using Xunit;

namespace POSSystem.Services.Product.Tests.Domain;

/// <summary>
/// Unit tests for Product aggregate root.
/// Follows Arrange-Act-Assert pattern and test naming conventions.
/// </summary>
public class ProductTests
{
    [Fact]
    public void Constructor_InitializesProductWithValidData()
    {
        // Arrange & Act
        var product = new ProductModel
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 29.99m,
            Sku = "TEST-001",
            CategoryId = Guid.NewGuid(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        Assert.NotEqual(Guid.Empty, product.Id);
        Assert.Equal("Test Product", product.Name);
        Assert.Equal(29.99m, product.Price);
        Assert.True(product.IsActive);
    }

    [Fact]
    public void Equality_ComparesById()
    {
        // Arrange
        var id = Guid.NewGuid();
        var product1 = new ProductModel { Id = id };
        var product2 = new ProductModel { Id = id };

        // Act & Assert
        Assert.Equal(product1, product2);
    }

    [Fact]
    public void Inequality_DifferentIds()
    {
        // Arrange
        var product1 = new ProductModel { Id = Guid.NewGuid() };
        var product2 = new ProductModel { Id = Guid.NewGuid() };

        // Act & Assert
        Assert.NotEqual(product1, product2);
    }
}


