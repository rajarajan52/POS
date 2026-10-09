using OrderModel = POSSystem.Domain.Domain.Models.Order;
using POSSystem.Domain.Domain.Models;

namespace POSSystem.Services.Order.Application.Services;

/// <summary>
/// State machine for order processing.
/// Implements proper state transitions following state pattern principles.
/// </summary>
public class OrderStateTransitionService
{
    public bool CanTransitionTo(OrderStatus currentStatus, OrderStatus targetStatus)
    {
        return (currentStatus, targetStatus) switch
        {
            (OrderStatus.Pending, OrderStatus.Confirmed) => true,
            (OrderStatus.Confirmed, OrderStatus.Processing) => true,
            (OrderStatus.Processing, OrderStatus.Completed) => true,
            (OrderStatus.Processing, OrderStatus.Cancelled) => true,
            (OrderStatus.Completed, OrderStatus.Cancelled) => true,
            _ => false
        };
    }

    public void TransitionOrder(OrderModel order, OrderStatus targetStatus)
    {
        if (!CanTransitionTo(order.Status, targetStatus))
        {
            throw new InvalidOperationException(
                $"Cannot transition from {order.Status} to {targetStatus}");
        }

        order.Status = targetStatus;

        if (targetStatus == OrderStatus.Completed)
        {
            order.CompletedAt = DateTime.UtcNow;
        }
    }
}


