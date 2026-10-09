namespace POSSystem.Domain.Application.DTOs;

/// <summary>
/// Data Transfer Object for Customer.
/// </summary>
public record CustomerDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    decimal LoyaltyPoints,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);


