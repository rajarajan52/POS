using POSSystem.Services.Payment.Application.Interfaces;

namespace POSSystem.Services.Payment.Infrastructure;

/// <summary>
/// Mock implementation of payment gateway for testing and development.
/// </summary>
public class MockPaymentGateway : IPaymentGateway
{
    public async Task<PaymentGatewayResponse> ProcessPaymentAsync(PaymentRequest request, CancellationToken cancellationToken)
    {
        // Simulate payment processing delay
        await Task.Delay(100, cancellationToken);

        // Mock success response
        return new PaymentGatewayResponse
        {
            IsSuccessful = true,
            TransactionId = Guid.NewGuid().ToString(),
            Message = "Payment processed successfully",
            Amount = request.Amount
        };
    }

    public async Task<PaymentGatewayResponse> RefundPaymentAsync(string transactionId, decimal amount, CancellationToken cancellationToken)
    {
        // Simulate refund processing delay
        await Task.Delay(100, cancellationToken);

        // Mock success response
        return new PaymentGatewayResponse
        {
            IsSuccessful = true,
            TransactionId = transactionId,
            Message = "Refund processed successfully",
            Amount = amount
        };
    }
}
