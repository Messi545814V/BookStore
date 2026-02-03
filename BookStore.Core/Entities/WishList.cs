namespace BookStore.Core.Entities;

public class WishList
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; }
    public DateTime DateAdded { get; set; } =  DateTime.UtcNow;
}