using POSSystem.Domain.Domain.Models;

namespace POSSystem.Services.Inventory.Domain.Events;

/// <summary>
/// Domain event raised when inventory stock is checked.
/// Follows Event Sourcing pattern for audit trail and event-driven architecture.
/// </summary>
public record StockCheckedEvent(
    Guid InventoryItemId,
    Guid ProductId,
    int RequestedQuantity,
    bool IsAvailable) : DomainEvent;

/// <summary>
/// Domain event raised when stock is reserved.
/// </summary>
public record StockReservedEvent(
    Guid InventoryItemId,
    Guid ProductId,
    int ReservedQuantity) : DomainEvent;

/// <summary>
/// Domain event raised when stock is insufficient.
/// </summary>
public record InsufficientStockEvent(
    Guid InventoryItemId,
    Guid ProductId,
    int AvailableQuantity,
    int RequestedQuantity) : DomainEvent;


