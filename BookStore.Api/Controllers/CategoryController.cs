using BookStore.Core.Entities;
using BookStore.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoryController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        var categories = await _categoryService.GetCategories();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetIdAsync(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null) return NotFound();
        return category;
    }

    [HttpPost]
    public async Task<ActionResult<Category>> PostAsync(Category category)
    {
        var newCategory = await _categoryService.AddCategoryAsync(category);
        if (newCategory != null)
            return CreatedAtAction(nameof(GetCategories), new { id = newCategory.Id }, newCategory);
        return BadRequest();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Category>> PutAsync(int id, Category category)
    {
        if (id != category.Id) return BadRequest();
        
        var updated = await _categoryService.UpdateCategoryAsync(category);
        if (updated == null) return NotFound();
        
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Category>> DeleteAsync(int id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return NoContent();
    }
}