using POSSystem.Domain.Application.DTOs;
using MediatR;

namespace POSSystem.Services.Product.Application.Queries;

/// <summary>
/// CQRS Query for retrieving a product by ID.
/// </summary>
public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;


