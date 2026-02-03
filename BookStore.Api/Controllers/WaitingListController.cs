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
public class WaitingListController : ControllerBase
{
    private readonly BookStoreContext _context;
    
    public WaitingListController(BookStoreContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize] // 🔒 Вимагає входу
    public async Task<ActionResult<List<WaitingItemDto>>> GetWaitingList()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized(); // Якщо ID не знайдено - помилка 401
        
        var items = await _context.WaitingItems
            .Where(w => w.UserId == userId)
            .Include(w => w.Book)
            .ThenInclude(b => b.Author)
            .OrderByDescending(s => s.DateAdded)
            .Select(w => new WaitingItemDto
            {
                BookId = w.Book.Id,
                Title = w.Book.Title,
                Author = w.Book.Author.Name,
                ImageUrl = w.Book.ImageUrl,
                Price = w.Book.Price,
                DateAdded = w.DateAdded,
                IsAvailable = w.Book.Stock > 0
            })
            .ToListAsync();
        
        return Ok(items);
    }

    [HttpPost("toggle/{bookId}")]
    [Authorize] // 🔒 Вимагає входу
    public async Task<IActionResult> Toggle(int bookId)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var existing = await _context.WaitingItems
            .FirstOrDefaultAsync(w => w.UserId == userId && w.BookId == bookId);

        if (existing != null)
        {
            _context.WaitingItems.Remove(existing);
            await _context.SaveChangesAsync();
            return Ok();
        }
        else
        {
            // userId.Value безпечно використовувати, бо ми перевірили на null вище
            var newItem = new WaitingItem { UserId = userId.Value, BookId = bookId };
            _context.WaitingItems.Add(newItem);
            await _context.SaveChangesAsync();
            return Ok(new { IsWaiting = true });
        }
    }

    [HttpGet("ids")]
    [AllowAnonymous] // 🔓 Дозволяємо гостям
    public async Task<ActionResult<List<int>>> GetWaitingIds()
    {
        var userId = GetUserId();
        
        // 👇 ЯКЩО ГІСТЬ (ID == null) - ПОВЕРТАЄМО ПУСТИЙ СПИСОК, А НЕ ПОМИЛКУ
        if (userId == null) 
        {
            return Ok(new List<int>());
        }

        var ids = await _context.WaitingItems
            .Where(w => w.UserId == userId)
            .Select(w => w.BookId)
            .ToListAsync();
        return Ok(ids);
    }


    private int? GetUserId()
    {
        var idString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (int.TryParse(idString, out int userId))
        {
            return userId;
        }

        return null;
    }
}