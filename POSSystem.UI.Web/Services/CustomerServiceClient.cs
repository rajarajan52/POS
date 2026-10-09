using POSSystem.Domain.Application.DTOs;
using System.Net.Http.Json;

namespace POSSystem.UI.Web.Services;

/// <summary>
/// HTTP client for Customer Service integration.
/// </summary>
public class CustomerServiceClient
{
    private readonly HttpClient _httpClient;

    public CustomerServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CustomerDto?> GetCustomerByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<CustomerDto>($"/api/customers/{id}");
        }
        catch
        {
            return null;
        }
    }
}


