using POSSystem.Domain.Application.DTOs;
using MediatR;

namespace POSSystem.Services.Product.Application.Commands;

/// <summary>
/// CQRS Command for creating a new product.
/// </summary>
public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    string Sku,
    Guid CategoryId) : IRequest<ProductDto>;


