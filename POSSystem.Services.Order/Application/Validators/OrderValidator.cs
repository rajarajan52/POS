using POSSystem.Domain.Domain.Models;
using OrderModel = POSSystem.Domain.Domain.Models.Order;
using FluentValidation;

namespace POSSystem.Services.Order.Application.Validators;

/// <summary>
/// Validator for Order.
/// </summary>
public class OrderValidator : AbstractValidator<OrderModel>
{
    public OrderValidator()
    {
        RuleFor(o => o.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(o => o.LineItems)
            .NotEmpty().WithMessage("Order must contain at least one line item");

        RuleFor(o => o.Total)
            .GreaterThan(0).WithMessage("Order total must be greater than zero");

        RuleForEach(o => o.LineItems)
            .ChildRules(lineItem =>
            {
                lineItem.RuleFor(li => li.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be greater than zero");

                lineItem.RuleFor(li => li.UnitPrice)
                    .GreaterThan(0).WithMessage("Unit price must be greater than zero");
            });
    }
}


