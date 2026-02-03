namespace BookStore.Core.DTOs;

public class UpdateQuantityDto
{
    public int BookId { get; set; }
    public int Delta { get; set; }
}