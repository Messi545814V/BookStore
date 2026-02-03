// BookStore.Infrastructure/Services/BookService.cs
using BookStore.Core.DTOs;
using BookStore.Core.Entities;
using BookStore.Core.Helpers;
using BookStore.Core.Interfaces;
using BookStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FuzzySharp;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace BookStore.Infrastructure.Services;

public class BookService : IBookService
{
    private readonly BookStoreContext _context;

    public BookService(BookStoreContext context)
    {
        _context = context;
    }
    
    public async Task<PageResult<BookSummaryDto>> GetAllBooksAsync(BookFilterDto filter)
    {
        var query = _context.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .AsQueryable();
        
        if (filter.CategoryId.HasValue)
            query = query.Where(b => b.CategoryId == filter.CategoryId.Value);

        if (filter.AuthorId.HasValue)
            query = query.Where(b => b.AuthorId == filter.AuthorId.Value);

        if (filter.MinPrice.HasValue)
            query = query.Where(b => b.Price >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue)
            query = query.Where(b => b.Price <= filter.MaxPrice.Value);
        
        List<BookSummaryDto> finalItems;
        int totalCount;
        
        
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var rawData = await query.ToListAsync(); 
            var searchTerm = TextNormalizer.Normalize(filter.SearchTerm);
            
            var searchResults = rawData
                .Select(b => new 
                {
                    Book = b,
                    Score = Fuzz.PartialRatio(searchTerm, b.SearchNormalized) 
                })
                .Where(x => x.Score >= 70)
                .OrderByDescending(x => x.Score)
                .Select(x => x.Book)
                .ToList();

            totalCount = searchResults.Count;
            
            int page = filter.Page ?? 1;
            int pageSize = filter.PageSize ?? 9;

            var pagedBooks = searchResults
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            finalItems = pagedBooks.Select(MapToSummary).ToList();
        }   
        else
        {
            query = filter.SortBy switch
            {
                "price_asc"  => query.OrderBy(b => (double)b.Price),
                "price_desc" => query.OrderByDescending(b => (double)b.Price),
                "year"       => query.OrderByDescending(b => b.Year),
                "title"      => query.OrderBy(b => b.Title),
                "rating"     => query.OrderByDescending(b => 
                    _context.BookRatings.Where(r => r.BookId == b.Id).Average(r => (double?)r.Rating) ?? 0),
                _            => query.OrderBy(b => b.Id)
            };

            totalCount = await query.CountAsync();

            int page = filter.Page ?? 1;
            int pageSize = filter.PageSize ?? 9;

            var pagedBooks = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            finalItems = pagedBooks.Select(MapToSummary).ToList();
        }

        return new PageResult<BookSummaryDto>
        {
            Items = finalItems,
            TotalCount = totalCount,
            CurrentPage = filter.Page ?? 1,
            PageSize = filter.PageSize ?? 9
        };
    }

    private BookSummaryDto MapToSummary(Book b)
    {
        return new BookSummaryDto
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author?.Name ?? "Невідомий",
            Price = b.Price,
            AuthorId = b.AuthorId,
            CategoryId = b.CategoryId,
            ImageUrl = b.ImageUrl,
            Genre = b.Genre,
            Stock = b.Stock,
            Year = b.Year,
            Language = b.Language,
            Rating = _context.BookRatings
                .Where(r => r.BookId == b.Id)
                .Average(r => (double?)r.Rating) ?? 0
        };
    }
    
    
    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    public async Task<BookDetailsDto?> GetBookByIdAsync(int id)
    {
        var book = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .Where(b => b.Id == id)
            .Select(b => new BookDetailsDto
            {
                Id = b.Id,
                Title = b.Title,
                AuthorName = b.Author.Name,
                CategoryName = b.Category.Name,
                Year = b.Year,
                Genre = b.Genre,
                Price = b.Price,
                Stock = b.Stock,
                AuthorId = b.AuthorId,
                CategoryId = b.CategoryId,
                ImageUrl = b.ImageUrl,
                Language = b.Language,
            })
            .FirstOrDefaultAsync();
            
        return book;
    }

    // 👇 ОСЬ ВИПРАВЛЕНІ МЕТОДИ 👇

    public async Task<Book> AddBookAsync(Book book)
    {
        // 1. Отримуємо імена автора та категорії для пошукового рядка
        var authorName = await _context.Authors
            .Where(a => a.Id == book.AuthorId)
            .Select(a => a.Name)
            .FirstOrDefaultAsync() ?? "";

        var categoryName = await _context.Categories
            .Where(c => c.Id == book.CategoryId)
            .Select(c => c.Name)
            .FirstOrDefaultAsync() ?? "";

        // 2. Формуємо повний рядок для пошуку (Назва + Жанр + Автор + Категорія)
        book.SearchNormalized = TextNormalizer.Normalize(
            $"{book.Title} {book.Genre} {authorName} {categoryName}"
        );

        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task UpdateBookAsync(Book book)
    {
        // 1. Те саме для оновлення
        var authorName = await _context.Authors
            .Where(a => a.Id == book.AuthorId)
            .Select(a => a.Name)
            .FirstOrDefaultAsync() ?? "";

        var categoryName = await _context.Categories
            .Where(c => c.Id == book.CategoryId)
            .Select(c => c.Name)
            .FirstOrDefaultAsync() ?? "";

        book.SearchNormalized = TextNormalizer.Normalize(
            $"{book.Title} {book.Genre} {authorName} {categoryName}"
        );

        _context.Books.Update(book);
        await _context.SaveChangesAsync();
    }


    public async Task DeleteBookAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<CategoryTreeDto>> GetUsedCategoryTreeAsync()
    {
        var allCategories = await _context.Categories.ToListAsync();
        var books = await _context.Books.ToListAsync();

        // CategoryId → Count
        var bookCounts = books
            .GroupBy(b => b.CategoryId)
            .ToDictionary(g => g.Key);

        // ParentId → Categories
        var lookup = allCategories.ToLookup(c => c.ParentId);

        List<CategoryTreeDto> BuildTree(int? parentId)
        {
            return lookup[parentId]
                .Select(c =>
                {
                    var node = new CategoryTreeDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ParentId = c.ParentId,
                        
                        Children = BuildTree(c.Id)
                    };

                   

                    return node;
                })
                // ❗ Показуємо тільки категорії, де є книги або у неї є підкатегорії з книгами
                
                .ToList();
        }

        return BuildTree(null);
    }


    
    public async Task<List<CategoryTreeDto>> GetCategoriesTreeAsync()
    {
        var categories = await _context.Categories.ToListAsync();

        var lookup = categories.ToLookup(c => c.ParentId);

        List<CategoryTreeDto> BuildTree(int? parentId)
        {
            return lookup[parentId]
                .Select(c => new CategoryTreeDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Children = BuildTree(c.Id)
                })
                .ToList();
        }
        return BuildTree(null);
    }

    public async Task<BookDetailsDto?> GetBookByIdWithReviewsAsync(int id)
    {
        return await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .Where(b => b.Id == id)
            .Select(b => new BookDetailsDto
            {
                Id = b.Id,
                Title = b.Title,
                AuthorName = b.Author.Name,
                CategoryName = b.Category.Name,
                Year = b.Year,
                Price = b.Price,
                Genre = b.Genre,
                Stock = b.Stock,
                AuthorId = b.AuthorId,
                CategoryId = b.CategoryId,
                ImageUrl = b.ImageUrl,
                Language = b.Language,
                Description = b.Description,

                AverageRating = _context.BookRatings
                    .Where(r => r.BookId == id)
                    .Select(r => (double?)r.Rating)
                    .Average() ?? 0,

                RatingsCount = _context.BookRatings
                    .Count(r => r.BookId == id)
            })
            .FirstOrDefaultAsync();
    }

}