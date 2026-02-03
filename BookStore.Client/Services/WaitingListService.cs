using System.Net.Http.Json;
using BookStore.Core.DTOs;
using BookStore.Core.Entities;

namespace BookStore.Client.Services;

public class WaitingListService
{
    private readonly HttpClient _http;
    public event Action? OnChange;

    public WaitingListService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<WaitingItemDto>> GetWaitingListAsync()
    {
        return await _http.GetFromJsonAsync<List<WaitingItemDto>>("api/waitinglist")
            ?? new List<WaitingItemDto>();
    }

    public async Task<HashSet<int>> GetWaitingListIdsAsync()
    {
        var ids = await _http.GetFromJsonAsync<List<int>>("api/waitinglist/ids");
        return ids?.ToHashSet() ?? new HashSet<int>();
    }
    
    public async Task ToggleAsync(int bookId)
    {
        await _http.PostAsync($"api/waitinglist/toggle/{bookId}", null);
        OnChange?.Invoke();
    }
}