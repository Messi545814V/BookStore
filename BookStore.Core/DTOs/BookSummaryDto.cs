namespace BookStore.Core.DTOs;

public class BookSummaryDto
{
    public int Id { get; set; }
    public int AuthorId { get; set; }
    public int CategoryId { get; set; }
    public string Title { get; set; } = string.Empty;   
    public string Author { get; set; } = string.Empty;
    public decimal Price { get; set; }
    
    public string ImageUrl { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Genre { get; set; } = string.Empty; 
    public int Stock { get; set; }     
    public int Year { get; set; }
    public string Language { get; set; } = string.Empty;
}