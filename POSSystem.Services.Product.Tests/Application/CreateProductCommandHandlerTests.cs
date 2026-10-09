using POSSystem.Domain.Application.DTOs;
using POSSystem.Domain.Application.Exceptions;
using POSSystem.Domain.Application.Interfaces;
using ProductModel = POSSystem.Domain.Domain.Models.Product;
using POSSystem.Domain.Domain.Models;
using POSSystem.Services.Product.Application.Commands;
using AutoMapper;
using FluentAssertions;
using Moq;
using Xunit;

namespace POSSystem.Services.Product.Tests.Application;

/// <summary>
/// Unit tests for CreateProductCommandHandler.
/// Tests CQRS command handler with repository pattern and dependency injection.
/// </summary>
public class CreateProductCommandHandlerTests
{
    private readonly Mock<IRepository<ProductModel, Guid>> _mockProductRepository;
    private readonly Mock<IRepository<Category, Guid>> _mockCategoryRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IRepository<ProductModel, Guid>>();
        _mockCategoryRepository = new Mock<IRepository<Category, Guid>>();
        _mockMapper = new Mock<IMapper>();
        _handler = new CreateProductCommandHandler(
            _mockProductRepository.Object,
            _mockCategoryRepository.Object,
            _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_CreatesProductSuccessfully()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new CreateProductCommand(
            "Test Product",
            "Test Description",
            29.99m,
            "TEST-001",
            categoryId);

        var createdProduct = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            Sku = command.Sku,
            CategoryId = categoryId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var expectedDto = new ProductDto(
            createdProduct.Id,
            createdProduct.Name,
            createdProduct.Description,
            createdProduct.Price,
            createdProduct.Sku,
            categoryId,
            true,
            createdProduct.CreatedAt,
            null);

        _mockCategoryRepository
            .Setup(r => r.ExistsAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockProductRepository
            .Setup(r => r.AddAsync(It.IsAny<ProductModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdProduct);

        _mockMapper.Setup(m => m.Map<ProductDto>(createdProduct))
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        result.Price.Should().Be(command.Price);
        _mockCategoryRepository.Verify(r => r.ExistsAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
        _mockProductRepository.Verify(r => r.AddAsync(It.IsAny<ProductModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidCategory_ThrowsNotFoundException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new CreateProductCommand(
            "Test Product",
            "Test Description",
            29.99m,
            "TEST-001",
            categoryId);

        _mockCategoryRepository
            .Setup(r => r.ExistsAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));

        _mockProductRepository.Verify(
            r => r.AddAsync(It.IsAny<ProductModel>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}


