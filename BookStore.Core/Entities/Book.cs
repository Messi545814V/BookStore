// BookStore.Core/Entities/Book.cs

using System.Text.Json.Serialization;

namespace BookStore.Core.Entities;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Year { get; set; }
    public int Stock { get; set; }
    public string Genre { get; set; } = string.Empty;

    // --- Зв'язок з Автором ---
    public int AuthorId { get; set; }
    
    
    [JsonIgnore]
    public Author? Author { get; set; }

    // --- Зв'язок з Категорією ---
    public int CategoryId { get; set; }
    
    [JsonIgnore]
    public Category? Category { get; set; }
    
    public string? Description { get; set; }
    
    [JsonIgnore]
    public List<BookRating> Ratings { get; set; } = new();

    // 💬 Коментарі (безліч)
    [JsonIgnore]
    public List<BookComment> Comments { get; set; } = new();
    public string ImageUrl { get; set; } = "";
    public string Language { get; set; } = "";
    
    
    public string? SearchNormalized { get; set; }
} 