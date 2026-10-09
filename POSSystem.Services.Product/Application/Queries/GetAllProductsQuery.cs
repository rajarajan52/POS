using POSSystem.Domain.Application.DTOs;
using MediatR;

namespace POSSystem.Services.Product.Application.Queries;

/// <summary>
/// CQRS Query for retrieving all products.
/// </summary>
public record GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>;


