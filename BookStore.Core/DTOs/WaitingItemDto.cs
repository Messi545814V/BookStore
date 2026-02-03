using BookStore.Core.Entities;

namespace BookStore.Core.DTOs;

public class WaitingItemDto
{
    public int BookId { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string ImageUrl { get; set; }
    public decimal Price { get; set; }
    public DateTime DateAdded { get; set; }
    
    public bool IsAvailable { get; set; }
}