using System.Text.Json.Serialization;

namespace BookStore.Core.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public int? ParentId { get; set; }
    [JsonIgnore]
    public Category? Parent { get; set; }
    [JsonIgnore]
    
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    [JsonIgnore]
    
    public ICollection<Book> Books { get; set; } =  new List<Book>();
}