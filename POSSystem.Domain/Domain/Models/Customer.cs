namespace POSSystem.Domain.Domain.Models;

/// <summary>
/// Represents a customer in the POS system.
/// </summary>
public class Customer : AggregateRoot
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public decimal LoyaltyPoints { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string GetFullName() => $"{FirstName} {LastName}".Trim();
}


