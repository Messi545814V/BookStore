using System.Net.Http.Json;
using BookStore.Core.DTOs;

namespace BookStore.Client.Services;

public class OrderService
{
    private readonly HttpClient _http;
    
    public OrderService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<OrderDto>> GetOrdersAsync()
    {
        return await _http.GetFromJsonAsync<List<OrderDto>>("api/order/my") 
               ?? new List<OrderDto>();
    }
}