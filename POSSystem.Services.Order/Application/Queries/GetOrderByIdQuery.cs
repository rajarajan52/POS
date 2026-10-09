using POSSystem.Domain.Application.DTOs;
using POSSystem.Domain.Application.Interfaces;
using OrderModel = POSSystem.Domain.Domain.Models.Order;
using POSSystem.Domain.Domain.Models;
using AutoMapper;
using MediatR;

namespace POSSystem.Services.Order.Application.Queries;

/// <summary>
/// CQRS Query for retrieving an order by ID.
/// </summary>
public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto?>;

/// <summary>
/// Handler for GetOrderByIdQuery.
/// </summary>
public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IRepository<OrderModel, Guid> _orderRepository;
    private readonly IMapper _mapper;

    public GetOrderByIdQueryHandler(IRepository<OrderModel, Guid> orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        return order is null ? null : _mapper.Map<OrderDto>(order);
    }
}


