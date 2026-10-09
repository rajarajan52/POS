using POSSystem.Domain.Application.DTOs;
using System.Text.Json;

namespace POSSystem.UI.Web.Services;

/// <summary>
/// HTTP client for Inventory Service integration.
/// </summary>
public class InventoryServiceClient
{
    private readonly HttpClient _httpClient;
    private const int RequestTimeoutSeconds = 30;

    public InventoryServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        // Set timeout to prevent indefinite hangs
        _httpClient.Timeout = TimeSpan.FromSeconds(RequestTimeoutSeconds);
    }

    public async Task<bool> CheckAvailabilityAsync(Guid productId, int quantity)
    {
        try
        {
            var request = new { productId, requestedQuantity = quantity };
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(RequestTimeoutSeconds));

            var response = await _httpClient.PostAsJsonAsync("/api/inventory/check-availability", request, cancellationToken: cts.Token);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return false;
            }

            // Try to deserialize to a typed response
            var result = JsonSerializer.Deserialize<InventoryCheckResponse>(json);
            return result?.IsAvailable ?? false;
        }
        catch (TaskCanceledException)
        {
            // Timeout - return false
            return false;
        }
        catch
        {
            // Any other error - return false (item not available)
            return false;
        }
    }
}

/// <summary>
/// Response model for inventory availability check.
/// </summary>
internal class InventoryCheckResponse
{
    public bool IsAvailable { get; set; }
}


