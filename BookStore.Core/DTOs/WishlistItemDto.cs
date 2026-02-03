namespace BookStore.Core.DTOs;

public class WishlistItemDto
{
    public int BookId { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string ImageUrl { get; set; }
    public decimal Price { get; set; }
    
    public bool IsInStock { get; set; }
    public double Rating { get; set; }  
    public int ReviewsCount { get; set; } 
    public bool IsTop { get; set; } 
}