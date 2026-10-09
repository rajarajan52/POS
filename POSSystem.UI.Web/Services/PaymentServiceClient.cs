using POSSystem.Domain.Application.DTOs;
using POSSystem.Domain.Domain.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace POSSystem.UI.Web.Services;

/// <summary>
/// HTTP client for Payment Service integration.
/// </summary>
public class PaymentServiceClient
{
    private readonly HttpClient _httpClient;
    private const int RequestTimeoutSeconds = 30;

    public PaymentServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        // Set timeout to prevent indefinite hangs
        _httpClient.Timeout = TimeSpan.FromSeconds(RequestTimeoutSeconds);
    }

    public async Task<PaymentDto?> ProcessPaymentAsync(Guid orderId, decimal amount, PaymentMethod method)
    {
        try
        {
            var request = new { orderId, amount, method = method.ToString() };
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(RequestTimeoutSeconds));

            var response = await _httpClient.PostAsJsonAsync("/api/payments/process", request, cts.Token);

            if (!response.IsSuccessStatusCode)
            {
                // Log error but don't throw - return null to indicate failure
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            var result = JsonSerializer.Deserialize<PaymentDto>(json);
            return result;
        }
        catch (TaskCanceledException)
        {
            // Timeout - return null instead of throwing
            return null;
        }
        catch (HttpRequestException ex)
        {
            throw new ServiceUnavailableException("Payment Service", ex);
        }
    }
}


