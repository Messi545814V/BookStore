namespace BookStore.Core.DTOs;

public class BookCommentDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = "";
    public string Comment { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public int? ParentId { get; set; }
    public bool IsApproved { get; set; }
    public List<BookCommentDto>? Replies { get; set; } = new();
    
    public bool IsAdmin { get; set; }
}