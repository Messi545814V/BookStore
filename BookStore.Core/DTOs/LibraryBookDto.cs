namespace BookStore.Core.DTOs;

public class LibraryBookDto
{
    public int BookID { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string ImageUrl { get; set; }
    public DateTime PurchaseDate { get; set; }
}