namespace POSSystem.Domain.Domain.Models;

/// <summary>
/// Represents a payment transaction.
/// </summary>
public class Payment : AggregateRoot
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string TransactionReference { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
    public string? ReceiptNumber { get; set; }
}

public enum PaymentMethod
{
    Cash,
    CreditCard,
    DebitCard,
    MobileWallet,
    Check
}

public enum PaymentStatus
{
    Pending,
    Authorized,
    Captured,
    Failed,
    Refunded,
    Voided,
    Completed
}


