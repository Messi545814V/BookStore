using System.Security.Claims;
using BookStore.Core.DTOs;
using BookStore.Core.Entities;
using BookStore.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Api.Controllers;

[ApiController]
[Route("api/books/{bookId}/rating")]
public class RatingController : ControllerBase
{
    private readonly BookStoreContext _context;

    public RatingController(BookStoreContext context)
    {
        _context = context;
    }

    // api/Controllers/RatingController.cs

    [HttpGet]
// Змінюємо тип повернення з BookRatingDto? на int
    public async Task<ActionResult<int>> GetRating(int bookId)
    {
        // Безпечне отримання ID користувача (для неавторизованих буде 0)
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = string.IsNullOrEmpty(userIdString) ? 0 : int.Parse(userIdString);

        // Одразу вибираємо тільки Rating (int). 
        // Якщо запис не знайдено, FirstOrDefaultAsync поверне 0 (дефолтне значення для int).
        var rating = await _context.BookRatings
            .Where(r => r.BookId == bookId && r.UserId == userId)
            .Select(r => r.Rating)
            .FirstOrDefaultAsync();

        // Тепер ми завжди повертаємо JSON (число 0 або оцінку), а не порожнє тіло.
        return Ok(rating);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> SetRating(int bookId, CreateRatingDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var existing = await _context.BookRatings
            .FirstOrDefaultAsync(r => r.BookId == bookId && r.UserId == userId);

        if (existing != null)
        {
            existing.Rating = dto.Rating;
        }
        else
        {
            _context.BookRatings.Add(new BookRating
            {
                BookId = bookId,
                UserId = userId,
                Rating = dto.Rating
            });
        }

        await _context.SaveChangesAsync();
        return Ok();
    }

    [Authorize]
    [HttpDelete] 
    public async Task<IActionResult> DeleteRating(int bookId)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
        
        var userId = int.Parse(userIdString);
        
        var rating = await _context.BookRatings
            .FirstOrDefaultAsync(r => r.BookId == bookId && r.UserId == userId);

        if (rating == null)
        {
            return NotFound("Оцінку не знайдено");
        }

        _context.BookRatings.Remove(rating);
        await _context.SaveChangesAsync();
        
        return Ok();
    }
}
