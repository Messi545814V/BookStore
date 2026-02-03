namespace BookStore.Core.Entities;

public class BookComment
{
    public int Id { get; set; }

    public int BookId { get; set; }
    public Book Book { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public string Comment { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public bool IsApproved { get; set; } = false;
    public int? ParentId { get; set; }
    public BookComment? Parent { get; set; }
    public List<BookComment> Replies { get; set; } = new();
}
