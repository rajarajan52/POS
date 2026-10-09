using POSSystem.Domain.Application.DTOs;
using POSSystem.Domain.Application.Interfaces;
using ProductModel = POSSystem.Domain.Domain.Models.Product;
using POSSystem.Domain.Domain.Models;
using AutoMapper;
using MediatR;

namespace POSSystem.Services.Product.Application.Commands;

/// <summary>
/// Handler for CreateProductCommand.
/// Implements product creation with validation and repository persistence.
/// </summary>
public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, ProductDto>
{
    private readonly IRepository<ProductModel, Guid> _productRepository;
    private readonly IRepository<Category, Guid> _categoryRepository;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(
        IRepository<ProductModel, Guid> productRepository,
        IRepository<Category, Guid> categoryRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Verify category exists
        var categoryExists = await _categoryRepository.ExistsAsync(request.CategoryId, cancellationToken);
        if (!categoryExists)
            throw new KeyNotFoundException($"Category with ID {request.CategoryId} not found");

        // Create new product
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Sku = request.Sku,
            Price = request.Price,
            CategoryId = request.CategoryId,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        // Persist to repository
        var createdProduct = await _productRepository.AddAsync(product, cancellationToken);

        // Map to DTO and return
        return _mapper.Map<ProductDto>(createdProduct);
    }
}
