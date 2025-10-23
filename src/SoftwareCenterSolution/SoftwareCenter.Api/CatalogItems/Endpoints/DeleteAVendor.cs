using System.Net.Http.Headers;
using Marten;
using Microsoft.AspNetCore.Http.HttpResults;
using SoftwareCenter.Api.CatalogItems.Entities;
using SoftwareCenter.Api.CatalogItems.Models;

namespace SoftwareCenter.Api.CatalogItems.Endpoints;

public class CatalogApiClient
{
    private readonly HttpClient _httpClient;

    public CatalogApiClient(string baseAddress, string accessToken)
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(baseAddress);
        // Set the Authorization header with the Bearer token for every request
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    public async Task<bool> DeleteCatalogItemAsync(int itemId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/vendors/{itemId}");

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                // Handle different error statuses
                string error = await response.Content.ReadAsStringAsync();
                return false;
            }
        }
        catch (HttpRequestException ex)
        {
            return false;
        }
    }
}
