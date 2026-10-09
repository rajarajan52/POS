using POSSystem.Domain.Domain.Models;
using ProductModel = POSSystem.Domain.Domain.Models.Product;
using FluentValidation;

namespace POSSystem.Services.Product.Application.Validators;

/// <summary>
/// Validator for Product creation and updates.
/// Follows Single Responsibility Principle - business rule validation.
/// </summary>
public class ProductValidator : AbstractValidator<ProductModel>
{
    public ProductValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters");

        RuleFor(p => p.Sku)
            .NotEmpty().WithMessage("SKU is required")
            .MaximumLength(50).WithMessage("SKU must not exceed 50 characters")
            .Matches(@"^[A-Z0-9-]+$").WithMessage("SKU must contain only uppercase letters, numbers, and hyphens");

        RuleFor(p => p.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero")
            .LessThan(999999.99m).WithMessage("Price cannot exceed 999,999.99");

        RuleFor(p => p.CategoryId)
            .NotEmpty().WithMessage("Category is required");
    }
}


