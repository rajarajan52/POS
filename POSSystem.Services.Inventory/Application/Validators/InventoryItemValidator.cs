using POSSystem.Domain.Domain.Models;
using FluentValidation;

namespace POSSystem.Services.Inventory.Application.Validators;

/// <summary>
/// Validator for InventoryItem.
/// </summary>
public class InventoryItemValidator : AbstractValidator<InventoryItem>
{
    public InventoryItemValidator()
    {
        RuleFor(i => i.ProductId)
            .NotEmpty().WithMessage("ProductId is required");

        RuleFor(i => i.QuantityOnHand)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity on hand must be non-negative");

        RuleFor(i => i.ReorderLevel)
            .GreaterThanOrEqualTo(0).WithMessage("Reorder level must be non-negative");

        RuleFor(i => i.ReorderQuantity)
            .GreaterThan(0).WithMessage("Reorder quantity must be positive");
    }
}


