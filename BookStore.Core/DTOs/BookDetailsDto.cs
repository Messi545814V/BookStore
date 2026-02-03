// BookStore.Core/DTOs/BookDetailsDto.cs

using System.Text.Json.Serialization;

namespace BookStore.Core.DTOs;



public class BookDetailsDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty; 
    public string AuthorName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    

    public int Year { get; set; }
    public decimal Price { get; set; }

    public string Genre { get; set; } = string.Empty;
    public int Stock { get; set; }

    public int AuthorId { get; set; }
    public int CategoryId { get; set; }

    public double AverageRating { get; set; }
    public int RatingsCount { get; set; }
    public string ImageUrl { get; set; } = "";
    public string Language { get; set; } = "";
}
