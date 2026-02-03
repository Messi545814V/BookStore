using BookStore.Core.Entities;
using BookStore.Infrastructure.Repositories;

namespace BookStore.Infrastructure.Services;

public class AuthorService
{
    private readonly IGenericRepository<Author> _authorRepository;

    public AuthorService(IGenericRepository<Author> authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task<IEnumerable<Author>> GetAuthors()
    {
        return await _authorRepository.GetAllAsync();
    }

    public async Task<Author?> GetAuthorById(int id)
    {
        return await _authorRepository.GetByIdAsync(id);
    }

    public async Task<Author> AddAuthor(Author author)
    {
        await _authorRepository.AddAsync(author);
        return author;
    }

    public async Task<Author> UpdateAuthor(Author author)
    {
        await _authorRepository.UpdateAsync(author);
        return author;
    }

    public async Task<Author?> DeleteAuthor(int id)
    {
        await _authorRepository.DeleteAsync(id);
        return null;
    }
}