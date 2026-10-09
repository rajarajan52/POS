namespace POSSystem.Domain.Domain.Models;

/// <summary>
/// Represents a line item in an order.
/// </summary>
public class OrderLineItem : Entity
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Discount { get; set; }
    public decimal Total => (UnitPrice * Quantity) - Discount;
}


