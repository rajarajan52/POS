using POSSystem.Domain.Domain.Models;

using CustomerModel = POSSystem.Domain.Domain.Models.Customer;

namespace POSSystem.Services.Customer.Application.Services;

/// <summary>
/// Loyalty program service for customer rewards management.
/// Follows Single Responsibility Principle for business logic.
/// </summary>
public class LoyaltyProgramService
{
    private const decimal PointsPerDollar = 1.0m;
    private const decimal RedemptionRate = 0.01m; // 1 point = $0.01

    public decimal CalculateEarnedPoints(decimal orderAmount)
    {
        return orderAmount * PointsPerDollar;
    }

    public decimal CalculateRedemptionValue(decimal points)
    {
        return points * RedemptionRate;
    }

    public void AddPoints(CustomerModel customer, decimal points)
    {
        customer.LoyaltyPoints += points;
        customer.UpdatedAt = DateTime.UtcNow;
    }

    public bool CanRedeemPoints(CustomerModel customer, decimal pointsToRedeem)
    {
        return customer.LoyaltyPoints >= pointsToRedeem;
    }

    public void RedeemPoints(CustomerModel customer, decimal points)
    {
        if (!CanRedeemPoints(customer, points))
        {
            throw new InvalidOperationException(
                $"Insufficient loyalty points. Available: {customer.LoyaltyPoints}, Requested: {points}");
        }

        customer.LoyaltyPoints -= points;
        customer.UpdatedAt = DateTime.UtcNow;
    }
}


