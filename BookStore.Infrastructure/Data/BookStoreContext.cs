using BookStore.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace BookStore.Infrastructure.Data;

public class BookStoreContext : DbContext
{
    public BookStoreContext(DbContextOptions<BookStoreContext> options) : base(options) { }
    
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<User> Users { get; set; }
    
    public DbSet<BookRating> BookRatings { get; set; }
    public DbSet<BookComment> BookComments { get; set; }

    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<WishList> WishLists { get; set; }
    public DbSet<WaitingItem>  WaitingItems { get; set; }
    public DbSet<BonusTransaction> BonusTransactions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Спочатку створюємо авторів та категорії
        modelBuilder.Entity<Author>().HasData(
            new Author { Id = 1, Name = "Robert C. Martin" },
            new Author { Id = 2, Name = "Andrew Hunt" }
        );

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Programming" }
        );
        modelBuilder.Entity<WaitingItem>()
            .HasIndex(o => new {o.UserId, o.BookId})
            .IsUnique();

        // 👇👇👇 ОСЬ ТУТ КЛЮЧОВЕ ВИПРАВЛЕННЯ 👇👇👇
        // Тепер заповнюємо книги, вказуючи ID автора та категорії
        modelBuilder.Entity<Book>().HasData(
            new Book
            {
                Id = 1,
                Title = "Clean Code",
                Price = 30,
                Year = 2008,
                Stock = 10,
                AuthorId = 1, // <-- Посилаємось на Robert C. Martin
                CategoryId = 1, // <-- Посилаємось на Programming
                Genre = "Programming"
            },
            new Book
            {
                Id = 2,
                Title = "The Pragmatic Programmer",
                Price = 25,
                Year = 1999,
                Stock = 15,
                AuthorId = 2, // <-- Посилаємось на Andrew Hunt
                CategoryId = 1, // <-- Посилаємось на Programming
                Genre = "Programming"
            }
        );
    }

}

