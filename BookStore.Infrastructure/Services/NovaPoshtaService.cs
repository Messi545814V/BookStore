using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace BookStore.Infrastructure.Services;

public class NovaPoshtaService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public NovaPoshtaService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["NovaPoshta:ApiKey"] ?? "a36c705968473ec66ffd0b165c0f71ff"; 
    }

    public async Task<string> SearchSettlements(string cityName)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null
        };
        var request = new
        {
            apiKey = _apiKey,
            modelName = "Address",
            calledMethod = "searchSettlements",
            methodProperties = new
            {
                CityName = cityName,
                Limit = "50",
                Page = "1"
            }
        };

        var response = await _httpClient.PostAsJsonAsync("https://api.novaposhta.ua/v2.0/json/", request, jsonOptions);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetWarehouses(string cityRef)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null
        };

        var request = new
        {
            apiKey = _apiKey,
            modelName = "Address",
            calledMethod = "getWarehouses",
            methodProperties = new
            {
                SettlementRef = cityRef, 
                
                Page = "1",
                Limit = "1000"
            }
        };

        var response = await _httpClient.PostAsJsonAsync("https://api.novaposhta.ua/v2.0/json/", request, jsonOptions);
        return await response.Content.ReadAsStringAsync();
    }
}