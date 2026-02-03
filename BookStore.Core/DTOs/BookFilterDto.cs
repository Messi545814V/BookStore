namespace BookStore.Core.DTOs;

public class BookFilterDto
{
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public string? Title { get; set; }
    public int? AuthorId { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinPrice { get; set; }
    
    public string? SortBy { get; set; }

    public int? Page { get; set; } = 1;
    public int? PageSize { get; set; } = 6;
}