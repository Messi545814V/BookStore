namespace BookStore.Core.DTOs;

public class CreateCommentDto
{
    public string Comment { get; set; } = "";
    public int? ParentId { get; set; }
}