namespace POSSystem.Services.Payment.Application.Services;

/// <summary>
/// Receipt generation service.
/// Follows Single Responsibility Principle for receipt formatting.
/// </summary>
public class ReceiptGenerationService
{
    public string GenerateReceiptNumber()
    {
        return $"RCP-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }

    public string FormatReceipt(PaymentReceiptData data)
    {
        var receipt = new System.Text.StringBuilder();
        receipt.AppendLine("================================");
        receipt.AppendLine("           RECEIPT");
        receipt.AppendLine("================================");
        receipt.AppendLine();
        receipt.AppendLine($"Receipt #: {data.ReceiptNumber}");
        receipt.AppendLine($"Date/Time: {data.ProcessedAt:yyyy-MM-dd HH:mm:ss}");
        receipt.AppendLine();
        receipt.AppendLine($"Order ID: {data.OrderId}");
        receipt.AppendLine($"Amount: ${data.Amount:F2}");
        receipt.AppendLine($"Method: {data.PaymentMethod}");
        receipt.AppendLine($"Status: {data.Status}");
        receipt.AppendLine();
        receipt.AppendLine("================================");
        receipt.AppendLine("   Thank you for your purchase!");
        receipt.AppendLine("================================");

        return receipt.ToString();
    }
}

public record PaymentReceiptData(
    string ReceiptNumber,
    Guid OrderId,
    decimal Amount,
    string PaymentMethod,
    string Status,
    DateTime ProcessedAt);


