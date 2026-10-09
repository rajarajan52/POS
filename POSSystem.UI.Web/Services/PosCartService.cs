using POSSystem.Domain.Application.DTOs;
using POSSystem.Domain.Domain.Models;

namespace POSSystem.UI.Web.Services;

/// <summary>
/// Shopping cart state management service.
/// Follows state management pattern for client-side POS operations.
/// </summary>
public class PosCartService
{
    private readonly List<CartItem> _items = [];

    public event Action? OnCartChanged;

    public IReadOnlyList<CartItem> GetItems() => _items.AsReadOnly();

    public void AddItem(ProductDto product, int quantity)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            _items.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = quantity
            });
        }

        OnCartChanged?.Invoke();
    }

    public void RemoveItem(Guid productId)
    {
        _items.RemoveAll(i => i.ProductId == productId);
        OnCartChanged?.Invoke();
    }

    public void UpdateQuantity(Guid productId, int quantity)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            item.Quantity = quantity;
            OnCartChanged?.Invoke();
        }
    }

    public decimal GetSubtotal() => _items.Sum(i => i.UnitPrice * i.Quantity);

    public void Clear()
    {
        _items.Clear();
        OnCartChanged?.Invoke();
    }
}

public class CartItem
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Total => UnitPrice * Quantity;
}


