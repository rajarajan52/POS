namespace POSSystem.Domain.Application.DTOs;

/// <summary>
/// Data Transfer Object for Order.
/// </summary>
public record OrderDto(
    Guid Id,
    Guid CustomerId,
    decimal Subtotal,
    decimal TaxAmount,
    decimal DiscountAmount,
    decimal Total,
    string Status,
    DateTime CreatedAt,
    DateTime? CompletedAt,
    List<OrderLineItemDto> LineItems);

public record OrderLineItemDto(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal Discount);


