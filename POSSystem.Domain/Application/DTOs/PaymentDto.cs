namespace POSSystem.Domain.Application.DTOs;

/// <summary>
/// Data Transfer Object for Payment.
/// </summary>
public record PaymentDto(
    Guid Id,
    Guid OrderId,
    decimal Amount,
    string Method,
    string Status,
    string TransactionReference,
    DateTime ProcessedAt,
    string? ReceiptNumber);


