using Microsoft.EntityFrameworkCore;
using BookStore.Infrastructure.Data;
using BookStore.Core.Entities;

namespace BookStore.Infrastructure.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly BookStoreContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(BookStoreContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }
     public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
     public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

     public async Task AddAsync(T entity)
     {
         await _dbSet.AddAsync(entity);
         await _context.SaveChangesAsync();
     }

     public async Task UpdateAsync(T entity)
     {
         _context.Entry(entity).State = EntityState.Modified;
         await _context.SaveChangesAsync();
     }


     

     public async Task DeleteAsync(int id)
     {
         var entity = await _dbSet.FindAsync(id);
         if (entity != null)
         {
             _dbSet.Remove(entity);
             await _context.SaveChangesAsync();
         }
     }
}