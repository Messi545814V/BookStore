using BookStore.Core.DTOs;
using BookStore.Core.Entities;

namespace BookStore.Core.Interfaces
{
    public interface IBookService
    {
        Task<PageResult<BookSummaryDto>> GetAllBooksAsync(BookFilterDto filter);

        Task<List<Category>> GetCategoriesAsync();

        Task<List<CategoryTreeDto>> GetCategoriesTreeAsync();
        Task<List<CategoryTreeDto>> GetUsedCategoryTreeAsync();

        Task<BookDetailsDto?> GetBookByIdAsync(int id);
        Task<Book> AddBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(int id);
        
        Task<BookDetailsDto?> GetBookByIdWithReviewsAsync(int id);

    }
}