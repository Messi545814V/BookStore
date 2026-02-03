using BookStore.Core.Entities;
using BookStore.Infrastructure.Repositories;

namespace BookStore.Infrastructure.Services;

public class CategoryService
{
    private readonly IGenericRepository<Category> _categoryRepository;

    public CategoryService(IGenericRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<Category>> GetCategories()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }

    public async Task<Category?> AddCategoryAsync(Category category)
    {
        await _categoryRepository.AddAsync(category);
        return category;
    }

    public async Task<Category?> UpdateCategoryAsync(Category category)
    {
        var existingCategory = await _categoryRepository.GetByIdAsync(category.Id);
        if (existingCategory == null)
        {
            return null;
        }

        existingCategory.Name = category.Name; // приклад, оновлюємо тільки потрібні поля
        await _categoryRepository.UpdateAsync(existingCategory);

        return existingCategory;
    }


    public async Task<Category?> DeleteCategoryAsync(int id)
    {
        await _categoryRepository.DeleteAsync(id);
        return null;
    }
}