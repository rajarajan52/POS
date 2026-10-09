using POSSystem.Domain.Application.DTOs;
using System.Net.Http.Json;

namespace POSSystem.UI.Web.Services;

/// <summary>
/// HTTP client for Product Service integration.
/// </summary>
public class ProductServiceClient
{
    private readonly HttpClient _httpClient;

    public ProductServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ProductDto>>("/api/products") ?? [];
        }
        catch (HttpRequestException ex)
        {
            throw new ServiceUnavailableException("Product Service", ex);
        }
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ProductDto>($"/api/products/{id}");
        }
        catch (HttpRequestException ex)
        {
            throw new ServiceUnavailableException("Product Service", ex);
        }
    }
}


