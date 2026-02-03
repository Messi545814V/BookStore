using System.Net.Http.Json;
using BookStore.Core.DTOs;
using BookStore.Core.Entities;

namespace BookStore.Client.Services;

public class AdminService
{
    private readonly HttpClient _http;
    
    public AdminService(HttpClient http)
    {
        _http = http;
    }

    public async Task<PageResult<BookSummaryDto>> GetBooksAsync(BookFilterDto filter)
    {
        var queryParams = new List<string>
        {
            $"page={filter.Page}",
            $"pageSize={filter.PageSize}"
        };

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            queryParams.Add($"searchTerm={Uri.EscapeDataString(filter.SearchTerm)}");

        if (filter.CategoryId.HasValue)
            queryParams.Add($"categoryId={filter.CategoryId.Value}");

        if (filter.AuthorId.HasValue)
            queryParams.Add($"authorId={filter.AuthorId.Value}");

        if (filter.MinPrice.HasValue)
            queryParams.Add($"minPrice={filter.MinPrice.Value}");

        if (filter.MaxPrice.HasValue)
            queryParams.Add($"maxPrice={filter.MaxPrice.Value}");

        if (!string.IsNullOrWhiteSpace(filter.SortBy))
            queryParams.Add($"sortBy={filter.SortBy}");

        var query = "api/Books?" + string.Join("&", queryParams);

        var result = await _http.GetFromJsonAsync<PageResult<BookSummaryDto>>(query);
        return result ?? new PageResult<BookSummaryDto>();
    }

    
    public async Task<BookDetailsDto?> GetBookDetailsAsync(int id)
    {
        return await _http.GetFromJsonAsync<BookDetailsDto?>($"api/books/{id}");
    }

    public async Task CreateBookAsync(BookSaveDto book)
    {
        var response = await _http.PostAsJsonAsync("api/books", book);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateBookAsync(BookSaveDto book)
    {
        var response = await _http.PutAsJsonAsync($"api/books/{book.Id}", book);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteBookAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/books/{id}");
        response.EnsureSuccessStatusCode();
    }

    // --- ЗАМОВЛЕННЯ ---

    public async Task<List<OrderSummaryDto>> GetOrdersAsync()
    {
        var orders = await _http.GetFromJsonAsync<List<OrderSummaryDto>>("api/order/all");
        return orders ?? new List<OrderSummaryDto>();
    }

    public async Task UpdateOrderStatusAsync(int orderId, string newStatus)
    {
        var response = await _http.PutAsJsonAsync($"api/order/{orderId}/status", newStatus);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Помилка: {error}");
        }
    }

    public async Task<List<Author>> GetAuthorsAsync()
    {
        var result = await _http.GetFromJsonAsync<List<Author>>("api/Author");
        return result ?? new List<Author>();
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        var result = await _http.GetFromJsonAsync<List<Category>>("api/Category");
        return result ?? new List<Category>();
    }

    public async Task<Author> CreateAuthorAsync(string name)
    {
        var response = await _http.PostAsJsonAsync("api/Author", new { Name = name });
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<Author>();
    }

    public async Task<Category> CreateCategoryAsync(string name)
    {
        var response = await _http.PostAsJsonAsync("api/Category", new { Name = name });
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<Category>();
    }

    public async Task<List<AdminCommentDto>> GetAllCommentsAsync()
    {
        return await _http.GetFromJsonAsync<List<AdminCommentDto>>("api/admin/comments") 
               ?? new List<AdminCommentDto>();
    }

    public async Task ApproveCommentAsync(int commentId)
    {
        await _http.PutAsync($"api/admin/comments/{commentId}/approve", null);
    }

    public async Task DeleteCommentAsync(int commentId) 
    {
        await _http.DeleteAsync($"api/admin/comments/{commentId}");
    }
}