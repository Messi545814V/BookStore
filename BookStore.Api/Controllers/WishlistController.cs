using System.Security.Claims;
using BookStore.Core.DTOs;
using BookStore.Core.Entities;
using BookStore.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly BookStoreContext _context;
    
    public WishlistController(BookStoreContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<WishlistItemDto>> GetWishlist()
    {
        var userId = GetUserId();

        var items = await _context.WishLists
            .Where(w => w.UserId == userId)
            .Include(w => w.Book)
            .ThenInclude(b => b.Author)
            .Include(w => w.Book)
            .OrderByDescending(w => w.DateAdded)
            .Select(w => new WishlistItemDto
            {
                BookId = w.Book.Id,
                Title = w.Book.Title,
                Author = w.Book.Author.Name,
                ImageUrl = w.Book.ImageUrl,
                Price = w.Book.Price,
                IsInStock = w.Book.Stock > 0,

                Rating = w.Book.Ratings.Any()
                    ? w.Book.Ratings.Average(r => (double)r.Rating)
                    : 0,

                ReviewsCount = w.Book.Ratings.Count,

                IsTop = w.Book.Ratings.Any() && w.Book.Ratings.Average(r => (double)r.Rating) >= 4.5
            })
            .ToListAsync();
        
        return Ok(items);
    }

    [HttpGet("ids")]
    [AllowAnonymous] 
    public async Task<ActionResult<List<int>>> GetWishlistIds()
    {
        var userId = GetUserId();
        
        if (userId == null)
        {
            return Ok(new List<int>());
        }

        var ids = await _context.WishLists
            .Where(w => w.UserId == userId)
            .Select(w => w.BookId)
            .ToListAsync();

        return Ok(ids);
    }

    [HttpPost("toggle/{bookId}")]
    public async Task<IActionResult> ToggleWishlist(int bookId)
    {
        var userId = GetUserId();
        
        var existingItem = await _context.WishLists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.BookId == bookId);

        if (existingItem != null)
        {
            _context.WishLists.Remove(existingItem);
            await _context.SaveChangesAsync();
            return Ok();
        }
        else
        {
            var newItem = new WishList()
            {
                UserId = userId.Value,
                BookId = bookId,
                DateAdded = DateTime.UtcNow
            };
            _context.WishLists.Add(newItem);
            await _context.SaveChangesAsync();
            return Ok(new { IsFavorite = true });
        }
    }


    private int? GetUserId()
    {
        var idString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (int.TryParse(idString, out int userId))
        {
            return userId;
        }

        return null; // Повертаємо null, якщо це гість
    }
}