using POSSystem.Domain.Application.DTOs;
using POSSystem.Domain.Application.Exceptions;
using POSSystem.Domain.Application.Interfaces;
using OrderModel = POSSystem.Domain.Domain.Models.Order;
using CustomerModel = POSSystem.Domain.Domain.Models.Customer;
using POSSystem.Domain.Domain.Models;
using POSSystem.Services.Order.Application.Commands;
using AutoMapper;
using MediatR;

namespace POSSystem.Services.Order.Application.CommandHandlers;

/// <summary>
/// Handler for CreateOrderCommand.
/// Implements transactional order creation with total calculation.
/// </summary>
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderDto>
{
    private readonly IRepository<OrderModel, Guid> _orderRepository;
    private readonly IRepository<Customer, Guid> _customerRepository;
    private readonly IMapper _mapper;

    public CreateOrderCommandHandler(
        IRepository<OrderModel, Guid> orderRepository,
        IRepository<CustomerModel, Guid> customerRepository,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Validate customer exists
        var customerExists = await _customerRepository.ExistsAsync(request.CustomerId, cancellationToken);
        if (!customerExists)
        {
            throw NotFoundException.For<CustomerModel>(request.CustomerId);
        }

        // Create order
        var order = new OrderModel
        {
            CustomerId = request.CustomerId,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            TaxAmount = request.TaxAmount,
            DiscountAmount = request.DiscountAmount
        };

        // Add line items
        foreach (var lineItem in request.LineItems)
        {
            order.AddLineItem(new OrderLineItem
            {
                ProductId = lineItem.ProductId,
                ProductName = lineItem.ProductName,
                UnitPrice = lineItem.UnitPrice,
                Quantity = lineItem.Quantity,
                Discount = lineItem.Discount
            });
        }

        // Calculate totals
        order.CalculateTotal();

        var createdOrder = await _orderRepository.AddAsync(order, cancellationToken);
        return _mapper.Map<OrderDto>(createdOrder);
    }
}


