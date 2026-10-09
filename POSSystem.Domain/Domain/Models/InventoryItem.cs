namespace POSSystem.Domain.Domain.Models;

/// <summary>
/// Represents inventory tracking for a product.
/// </summary>
public class InventoryItem : AggregateRoot
{
    public Guid ProductId { get; set; }
    public int QuantityOnHand { get; set; }
    public int ReorderLevel { get; set; }
    public int ReorderQuantity { get; set; }
    public DateTime LastUpdated { get; set; }

    public bool IsLowStock() => QuantityOnHand <= ReorderLevel;

    public bool CanFulfillOrder(int quantity) => QuantityOnHand >= quantity;
}


