using POSSystem.Domain.Application.DTOs;
using POSSystem.Domain.Application.Interfaces;
using ProductModel = POSSystem.Domain.Domain.Models.Product;
using POSSystem.Domain.Domain.Models;
using AutoMapper;
using MediatR;

namespace POSSystem.Services.Product.Application.Queries;

/// <summary>
/// Handler for GetAllProductsQuery.
/// </summary>
public class GetAllProductsQueryHandler : IQueryHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IRepository<ProductModel, Guid> _productRepository;
    private readonly IMapper _mapper;

    public GetAllProductsQueryHandler(IRepository<ProductModel, Guid> productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }
}


