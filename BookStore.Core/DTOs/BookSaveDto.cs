using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.DTOs;

public class BookSaveDto
{
    public int Id { get; set; }             
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    [Range(1300, 2026, ErrorMessage = "Рік має бути між 1300 та 2026")]
    public int Year { get; set; }
    public int Stock { get; set; }
    public string? Description { get; set; }
    public string? Language { get; set; }
    public int PageCount { get; set; }
    public string Genre { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public int CategoryId { get; set; }
    public string ImageUrl { get; set; } = "";
}