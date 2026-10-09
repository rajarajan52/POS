using POSSystem.Domain.Application.DTOs;
using System.Net.Http.Json;
using System.Text.Json;

namespace POSSystem.UI.Web.Services;

/// <summary>
/// HTTP client for Order Service integration.
/// </summary>
public class OrderServiceClient
{
    private readonly HttpClient _httpClient;
    private const int RequestTimeoutSeconds = 30;

    public OrderServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        // Set timeout to prevent indefinite hangs
        _httpClient.Timeout = TimeSpan.FromSeconds(RequestTimeoutSeconds);
    }

    public async Task<OrderDto?> CreateOrderAsync(OrderDto order)
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(RequestTimeoutSeconds));

            var response = await _httpClient.PostAsJsonAsync("/api/orders", order, cancellationToken: cts.Token);

            if (!response.IsSuccessStatusCode)
            {
                // Return null for non-success responses instead of throwing
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            var result = JsonSerializer.Deserialize<OrderDto>(json);
            return result;
        }
        catch (TaskCanceledException)
        {
            // Timeout - return null instead of throwing
            return null;
        }
        catch (HttpRequestException ex)
        {
            throw new ServiceUnavailableException("Order Service", ex);
        }
    }
}


