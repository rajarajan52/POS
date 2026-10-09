using POSSystem.Domain.Application.Interfaces;
using ProductModel = POSSystem.Domain.Domain.Models.Product;
using POSSystem.Domain.Domain.Models;
using POSSystem.Services.Product.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace POSSystem.Services.Product.Tests.Infrastructure;

/// <summary>
/// Unit tests for EfCoreRepository.
/// Tests Repository pattern implementation with Entity Framework Core.
/// </summary>
public class EfCoreRepositoryTests
{
    private ProductServiceDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ProductServiceDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ProductServiceDbContext(options);
    }

    [Fact]
    public async Task AddAsync_WithValidEntity_AddsToDatabase()
    {
        // Arrange
        using var context = GetDbContext();
        var repository = new EfCoreRepository<ProductModel>(context);
        var product = new ProductModel
        {
            Name = "Test Product",
            Sku = "TEST-001",
            Price = 29.99m,
            CategoryId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await repository.AddAsync(product);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        var retrieved = await repository.GetByIdAsync(result.Id);
        retrieved.Should().NotBeNull();
        retrieved?.Name.Should().Be("Test Product");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsEntity()
    {
        // Arrange
        using var context = GetDbContext();
        var repository = new EfCoreRepository<ProductModel>(context);
        var product = new ProductModel
        {
            Name = "Test Product",
            Sku = "TEST-001",
            Price = 29.99m,
            CategoryId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };
        await repository.AddAsync(product);

        // Act
        var result = await repository.GetByIdAsync(product.Id);

        // Assert
        result.Should().NotBeNull();
        result?.Name.Should().Be("Test Product");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        using var context = GetDbContext();
        var repository = new EfCoreRepository<ProductModel>(context);

        // Act
        var result = await repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleEntities_ReturnsAll()
    {
        // Arrange
        using var context = GetDbContext();
        var repository = new EfCoreRepository<ProductModel>(context);
        var product1 = new ProductModel { Name = "Product 1", Sku = "SKU-001", CategoryId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };
        var product2 = new ProductModel { Name = "Product 2", Sku = "SKU-002", CategoryId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };

        await repository.AddAsync(product1);
        await repository.AddAsync(product2);

        // Act
        var results = await repository.GetAllAsync();

        // Assert
        results.Should().HaveCount(2);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_RemovesEntity()
    {
        // Arrange
        using var context = GetDbContext();
        var repository = new EfCoreRepository<ProductModel>(context);
        var product = new ProductModel { Name = "Test Product", Sku = "SKU-001", CategoryId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };
        await repository.AddAsync(product);

        // Act
        var result = await repository.DeleteAsync(product.Id);

        // Assert
        result.Should().BeTrue();
        var retrieved = await repository.GetByIdAsync(product.Id);
        retrieved.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_WithValidId_ReturnsTrue()
    {
        // Arrange
        using var context = GetDbContext();
        var repository = new EfCoreRepository<ProductModel>(context);
        var product = new ProductModel { Name = "Test Product", Sku = "SKU-001", CategoryId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };
        await repository.AddAsync(product);

        // Act
        var result = await repository.ExistsAsync(product.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithInvalidId_ReturnsFalse()
    {
        // Arrange
        using var context = GetDbContext();
        var repository = new EfCoreRepository<ProductModel>(context);

        // Act
        var result = await repository.ExistsAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }
}


