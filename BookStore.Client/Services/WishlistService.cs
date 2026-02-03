using System.Net.Http.Json;
using BookStore.Core.DTOs;

namespace BookStore.Client.Services;

public class WishlistService
{
    private readonly HttpClient _http;
    
    public event Action? OnChange;
    
    public WishlistService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<WishlistItemDto>> GetWishlistAsync()
    {
        return await _http.GetFromJsonAsync<List<WishlistItemDto>>("api/wishlist")
               ?? new List<WishlistItemDto>();
    }

    public async Task<HashSet<int>> GetWishListIds()
    {
        var ids = await _http.GetFromJsonAsync<List<int>>("api/Wishlist/ids");
        return ids?.ToHashSet() ?? new HashSet<int>();
    }
    
    public async Task ToggleWishlistAsync(int bookId)
    {
        var response = await _http.PostAsync($"api/wishlist/toggle/{bookId}", null);
        
        if (response.IsSuccessStatusCode)
        {
            OnChange?.Invoke(); // Сповіщаємо про зміни
        }
    }
}