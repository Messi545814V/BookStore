using BookStore.Core.DTOs;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Text.Json;

namespace BookStore.Client.Services;

public class CartService
{
    private readonly HttpClient _http;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly IJSRuntime _js;

    public static List<CartItemDto> Items { get; private set; } = new();
    public static event Action? OnChange;

    private const string LocalStorageKey = "guest_cart";

    public CartService(HttpClient http, AuthenticationStateProvider authStateProvider, IJSRuntime js)
    {
        _http = http;
        _authStateProvider = authStateProvider;
        _js = js;
    }
    
    private async Task<bool> IsUserAuthenticated()
    {
        var state = await _authStateProvider.GetAuthenticationStateAsync();
        return state.User.Identity?.IsAuthenticated ?? false;
    }

    public async Task GetCartItemsAsync()
    {
        if (await IsUserAuthenticated())
        {
            try
            {
                var result = await _http.GetFromJsonAsync<List<CartItemDto>>("api/cart");
                Items = result ?? new List<CartItemDto>();
            }
            catch (Exception e)
            {
                Console.WriteLine($"CartService API Error: {e.Message}");
                Items = new List<CartItemDto>();
            }
        }
        else
        {
            // --- РЕЖИМ ГОСТЯ (LocalStorage) ---
            try 
            {
                var json = await _js.InvokeAsync<string>("localStorage.getItem", LocalStorageKey);
                if (!string.IsNullOrEmpty(json))
                {
                    Items = JsonSerializer.Deserialize<List<CartItemDto>>(json) ?? new List<CartItemDto>();
                }
                else
                {
                    Items = new List<CartItemDto>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"CartService LocalStorage Error: {e.Message}");
                Items = new List<CartItemDto>();
            }
        }
        NotifyStateChanged();
    }

    public async Task AddToCart(BookSummaryDto book)
    {
        if (await IsUserAuthenticated())
        {
            // API
            var cartItem = new CartItemDto { BookId = book.Id, Quantity = 1 };
            var response = await _http.PostAsJsonAsync("api/cart/add", cartItem);
            response.EnsureSuccessStatusCode();
            await GetCartItemsAsync();
        }
        else
        {
            // LOCAL
            var existingItem = Items.FirstOrDefault(i => i.BookId == book.Id);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                Items.Add(new CartItemDto
                {
                    BookId = book.Id,
                    BookTitle = book.Title,
                    ImageUrl = book.ImageUrl,
                    Price = book.Price,
                    Quantity = 1
                });
            }
            await SaveLocalCart();
            NotifyStateChanged();
        }
    }

    public async Task RemoveFromCartAsync(int id)
    {
        if (await IsUserAuthenticated())
        {
            // API
            var response = await _http.DeleteAsync($"api/cart/{id}");
            response.EnsureSuccessStatusCode();
            await GetCartItemsAsync();
        }
        else
        {
            // LOCAL
            var item = Items.FirstOrDefault(i => i.BookId == id);
            if (item != null)
            {
                Items.Remove(item);
                await SaveLocalCart();
                NotifyStateChanged();
            }
        }
    }

    public async Task UpdateQuantity(int bookId, int delta)
    {
        if (await IsUserAuthenticated())
        {
            // API
            var dto = new { BookId = bookId, Delta = delta };
            var resp = await _http.PostAsJsonAsync("api/cart/update", dto);
            resp.EnsureSuccessStatusCode();
            await GetCartItemsAsync();
        }
        else
        {
            // LOCAL
            var item = Items.FirstOrDefault(i => i.BookId == bookId);
            if (item != null)
            {
                item.Quantity += delta;
                if (item.Quantity <= 0)
                {
                    Items.Remove(item);
                }
                await SaveLocalCart();
                NotifyStateChanged();
            }
        }
    }

    public async Task<int> PlaceCheckoutAsync(CheckoutOrderDto dto)
    {
        // Оформлення доступне тільки авторизованим (перевірка на рівні UI/Controller),
        // але про всяк випадок тут теж API виклик.
        var response = await _http.PostAsJsonAsync("api/order/checkout", dto);
        response.EnsureSuccessStatusCode();
        
        var orderId = await response.Content.ReadFromJsonAsync<int>();
        
        Items.Clear();
        NotifyStateChanged();

        return orderId;
    }

    // --- Спеціальні методи ---

    private async Task SaveLocalCart()
    {
        var json = JsonSerializer.Serialize(Items);
        await _js.InvokeVoidAsync("localStorage.setItem", LocalStorageKey, json);
    }


    public async Task SyncGuestCartToApi()
    {
        try
        {
            var json = await _js.InvokeAsync<string>("localStorage.getItem", LocalStorageKey);
            if (string.IsNullOrEmpty(json)) return;

            var localItems = JsonSerializer.Deserialize<List<CartItemDto>>(json);
            if (localItems == null || !localItems.Any()) return;
            
            foreach (var item in localItems)
            {
                var dto = new CartItemDto { BookId = item.BookId, Quantity = item.Quantity };
                await _http.PostAsJsonAsync("api/cart/add", dto);
            }
            
            await _js.InvokeVoidAsync("localStorage.removeItem", LocalStorageKey);
            
            await GetCartItemsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Sync Error: {ex.Message}");
        }
    }

    private void NotifyStateChanged()
    {
        Console.WriteLine($"CartService: State changed. Items: {Items.Count}");
        OnChange?.Invoke();
    }
}