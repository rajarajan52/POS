namespace POSSystem.Services.Payment.Application.Interfaces;

/// <summary>
/// Interface for payment gateway abstraction.
/// Supports multiple payment methods and providers.
/// </summary>
public interface IPaymentGateway
{
    /// <summary>
    /// Process a payment transaction.
    /// </summary>
    Task<PaymentGatewayResponse> ProcessPaymentAsync(PaymentRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Refund a previously processed payment.
    /// </summary>
    Task<PaymentGatewayResponse> RefundPaymentAsync(string transactionId, decimal amount, CancellationToken cancellationToken);
}

/// <summary>
/// Request model for payment processing.
/// </summary>
public class PaymentRequest
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public string CardToken { get; set; }
}

/// <summary>
/// Response model from payment gateway.
/// </summary>
public class PaymentGatewayResponse
{
    public bool IsSuccessful { get; set; }
    public string TransactionId { get; set; }
    public string Message { get; set; }
    public decimal Amount { get; set; }
}
