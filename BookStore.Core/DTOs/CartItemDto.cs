namespace BookStore.Core.DTOs;

public class CartItemDto
{
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }
}