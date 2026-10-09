using POSSystem.Domain.Application.DTOs;
using POSSystem.Domain.Domain.Models;
using MediatR;

namespace POSSystem.Services.Order.Application.Commands;

/// <summary>
/// CQRS Command for creating a new order.
/// </summary>
public record CreateOrderCommand(
    Guid CustomerId,
    List<OrderLineItemDto> LineItems,
    decimal TaxAmount = 0,
    decimal DiscountAmount = 0) : IRequest<OrderDto>;


