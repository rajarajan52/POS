namespace POSSystem.Domain.Domain.Models;

/// <summary>
/// Represents an order in the POS system.
/// </summary>
public class Order : AggregateRoot
{
    public Guid CustomerId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public List<OrderLineItem> LineItems { get; set; } = [];

    /// <summary>
    /// Adds a line item to the order.
    /// </summary>
    public void AddLineItem(OrderLineItem lineItem)
    {
        LineItems.Add(lineItem);
    }

    /// <summary>
    /// Calculates the total amount for the order including tax and discount.
    /// </summary>
    public void CalculateTotal()
    {
        Subtotal = LineItems.Sum(li => li.Quantity * li.UnitPrice);
        Total = Subtotal + TaxAmount - DiscountAmount;
    }
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Processing,
    Completed,
    Cancelled,
    Failed
}
