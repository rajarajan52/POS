using POSSystem.Domain.Application.DTOs;
using POSSystem.Domain.Domain.Models;

namespace POSSystem.UI.Web.Services;

/// <summary>
/// Checkout orchestration service.
/// Coordinates order creation, payment, and inventory updates.
/// </summary>
public class PosCheckoutService
{
    private readonly OrderServiceClient _orderService;
    private readonly PaymentServiceClient _paymentService;
    private readonly InventoryServiceClient _inventoryService;

    public PosCheckoutService(
        OrderServiceClient orderService,
        PaymentServiceClient paymentService,
        InventoryServiceClient inventoryService)
    {
        _orderService = orderService;
        _paymentService = paymentService;
        _inventoryService = inventoryService;
    }

    public async Task<CheckoutResult> CheckoutAsync(
        Guid customerId,
        List<CartItem> items,
        PaymentMethod paymentMethod,
        decimal taxAmount = 0,
        decimal discountAmount = 0)
    {
        try
        {
            // Validate inputs
            if (items == null || items.Count == 0)
            {
                return new CheckoutResult
                {
                    Success = false,
                    Message = "Cart is empty"
                };
            }

            // Check inventory for all items
            foreach (var item in items)
            {
                var isAvailable = await _inventoryService.CheckAvailabilityAsync(item.ProductId, item.Quantity);
                if (!isAvailable)
                {
                    return new CheckoutResult
                    {
                        Success = false,
                        Message = $"Product {item.ProductName} is not available in requested quantity"
                    };
                }
            }

            // Create order
            var lineItems = items.Select(i => new OrderLineItemDto(
                i.ProductId,
                i.ProductName,
                i.UnitPrice,
                i.Quantity,
                0)).ToList();

            // Calculate total
            var subtotal = items.Sum(i => i.UnitPrice * i.Quantity);
            var total = subtotal + taxAmount - discountAmount;

            var orderDto = new OrderDto(
                Guid.NewGuid(),
                customerId,
                subtotal,
                taxAmount,
                discountAmount,
                total,
                "Pending",
                DateTime.UtcNow,
                null,
                lineItems);

            var createdOrder = await _orderService.CreateOrderAsync(orderDto);

            // Check if order creation failed
            if (createdOrder == null)
            {
                return new CheckoutResult
                {
                    Success = false,
                    Message = "Failed to create order. Please try again."
                };
            }

            // Process payment
            var payment = await _paymentService.ProcessPaymentAsync(
                createdOrder.Id,
                createdOrder.Total,
                paymentMethod);

            if (payment != null)
            {
                return new CheckoutResult
                {
                    Success = true,
                    OrderId = createdOrder.Id,
                    ReceiptNumber = payment.ReceiptNumber,
                    Message = "Checkout completed successfully"
                };
            }

            return new CheckoutResult
            {
                Success = false,
                Message = "Payment processing failed. Please try again."
            };
        }
        catch (ServiceUnavailableException ex)
        {
            return new CheckoutResult
            {
                Success = false,
                Message = $"Service error: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            return new CheckoutResult
            {
                Success = false,
                Message = $"Checkout failed: {ex.Message}"
            };
        }
    }
}

public class CheckoutResult
{
    public bool Success { get; set; }
    public Guid OrderId { get; set; }
    public string? ReceiptNumber { get; set; }
    public string Message { get; set; } = string.Empty;
}


