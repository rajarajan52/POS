namespace POSSystem.Domain.Application.DTOs;

/// <summary>
/// Data Transfer Object for Product.
/// </summary>
public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Sku,
    Guid CategoryId,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);


