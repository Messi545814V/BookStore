using System.Net.Http.Json;
using System.Net.Quic;
using BookStore.Core.DTOs;
using BookStore.Core.Entities;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace BookStore.Client.Services;

public class ClientBookService
{
    private readonly HttpClient _http;
    
    public ClientBookService(HttpClient http)
    {
        _http = http;
    }

    public async Task<PageResult<BookSummaryDto>> GetBooksAsync(BookFilterDto filter)
    {
        var query = new List<string>();

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            query.Add($"searchTerm={filter.SearchTerm}");
        }

        if (filter.CategoryId.HasValue)
        {
            query.Add($"categoryId={filter.CategoryId}");
        }

        if (filter.MinPrice.HasValue)
        {
            query.Add($"minPrice={filter.MinPrice}");
        }

        if (filter.MaxPrice.HasValue)
        {
            query.Add($"maxPrice={filter.MaxPrice}");
        }

        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            query.Add($"sortBy={filter.SortBy}");
        }
        
        query.Add($"page={filter.Page}");
        query.Add($"pageSize={filter.PageSize}");

        var url = $"api/books?{string.Join("&", query)}";

        try
        {
            var result = await _http.GetFromJsonAsync<PageResult<BookSummaryDto>>(url);
            return result ??  new PageResult<BookSummaryDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching books: {ex.Message}");
            return new PageResult<BookSummaryDto>();
        }
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Category>>($"api/books/categories") ?? new List<Category>();
        }
        catch (Exception e)
        {
            return new List<Category>();
        }
    }

    public async Task<List<CategoryTreeDto>> GetCategoriesTreeAsync()
    {
        var flat = await _http.GetFromJsonAsync<List<Category>>("api/books/categories");

        if (flat == null)
            return new List<CategoryTreeDto>();

        // Формуємо дерево
        var lookup = flat.ToDictionary(c => c.Id, c => new CategoryTreeDto()
        {
            Id = c.Id,
            Name = c.Name,
            ParentId = c.ParentId,
            Children = new List<CategoryTreeDto>()
        });

        List<CategoryTreeDto> roots = new();

        foreach (var c in lookup.Values)
        {
            if (c.ParentId == null)
            {
                roots.Add(c);
            }
            else if (lookup.ContainsKey(c.ParentId.Value))
            {
                lookup[c.ParentId.Value].Children.Add(c);
            }
        }

        return roots.OrderBy(r => r.Name).ToList();
    }
    
    public async Task<List<CategoryTreeDto>> GetUsedCategoryTreeAsync()
    {
        return await _http.GetFromJsonAsync<List<CategoryTreeDto>>("api/books/tree-used") 
               ?? new();
    }

    public async Task<BookDetailsDto?> GetBookByIdAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<BookDetailsDto>($"api/books/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при отриманні книги з ID {id}: {ex.Message}");
            return null;
        }
    }
}