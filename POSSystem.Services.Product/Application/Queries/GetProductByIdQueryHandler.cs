using POSSystem.Domain.Application.DTOs;
using POSSystem.Domain.Application.Interfaces;
using ProductModel = POSSystem.Domain.Domain.Models.Product;
using POSSystem.Domain.Domain.Models;
using AutoMapper;
using MediatR;

namespace POSSystem.Services.Product.Application.Queries;

/// <summary>
/// Handler for GetProductByIdQuery.
/// Implements CQRS Query Handler pattern for read operations.
/// </summary>
public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IRepository<ProductModel, Guid> _productRepository;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IRepository<ProductModel, Guid> productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        return product is null ? null : _mapper.Map<ProductDto>(product);
    }
}


