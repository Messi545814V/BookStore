using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStore.Core.DTOs;
using BookStore.Core.Entities;
using BookStore.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using BCrypt.Net;

namespace BookStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly BookStoreContext _context;
    
    public ProfileController(BookStoreContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        // 1. Отримуємо ID як рядок з токена
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // 2. Перетворюємо в число (ВАЖЛИВО:FindAsync впаде, якщо передати рядок)
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId)) 
            return Unauthorized();
        
        // 3. Шукаємо користувача в БД
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound("User not found in DB");
        
        // 4. Рахуємо кількість замовлень (ефективно, порівнюючи int)
        var ordersCount = await _context.Orders
            .CountAsync(o => o.UserId == userId);

        // 5. Рахуємо кількість товарів у вішлісті
        // (Переконайтеся, що у вас є DbSet<WishListItem> WishListItems в контексті)
        var wishListCount = await _context.WishLists
            .CountAsync(w => w.UserId == userId);

        // 6. Визначаємо рівень (Логіка: Читач < 5000 < Книгоман < 15000 < Експерт)
        string levelName = "Читач";
        if (user.TotalSpent >= 15000) levelName = "Експерт";
        else if (user.TotalSpent >= 5000) levelName = "Книгоман";

        // 7. Формуємо DTO
        var dto = new UserProfileDto
        {
            Id = userIdString,
            // Пріоритет: Повне ім'я з БД -> Логін -> Email з токена
            FullName = !string.IsNullOrEmpty(user.FullName) ? user.FullName : (user.Username ?? "Користувач"),
            PhoneNumber = user.PhoneNumber ?? "",
            Email = user.Email ?? "",
            Surname = user.Surname ?? "",
            
            BonusBalance = (int)user.BonusBalance,
            UserLevel = levelName,
            
            OrderCount = ordersCount,
            TotalSpent = user.TotalSpent,
            WishListCount = wishListCount // Тепер тут реальна цифра
            
        };
        
        return Ok(dto);
    }

    [HttpGet("library")]
    public async Task<ActionResult<List<LibraryBookDto>>> GetLibraryBooks()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

        var library = await _context.Orders
            .Where(o => o.UserId == userId && o.Status == "Completed")
            .SelectMany(o => o.OrderItems)
            .Select(oi => new LibraryBookDto
            {
                BookID = oi.BookId,
                Title = oi.Book.Title,
                Author = oi.Book.Author.Name,
                ImageUrl = oi.Book.ImageUrl,
                PurchaseDate = oi.Order.CreatedAt
            })
            .Distinct()
            .ToListAsync();
        
        return Ok(library);
    }

    [HttpGet("bonuses/history")]
    public async Task<ActionResult<List<BonusTransactionDto>>> GetBonusHistory()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdString, out int userId)) return Unauthorized();
        
        var transactions = await _context.BonusTransactions
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.Date)
            .Select(b => new BonusTransactionDto
            {
                Id = b.Id,
                Date = b.Date,
                Amount = b.Amount,
                Description = b.Description,
                OrderId = b.OrderId,
            })
            .ToListAsync();
        
        return Ok(transactions);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateDto dto)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdString, out int userId)) return Unauthorized();
        
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound("User not found in DB");

        user.FullName = dto.FullName;
        user.PhoneNumber = dto.Phone;
        user.Email = dto.Email;
        user.Surname = dto.Surname;
        
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdString, out int userId)) return Unauthorized();
        
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound("User not found in DB");

        if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
        {
            return BadRequest(new { Message = "Старий пароль невірний" });
        }
        
        string newHash= BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        
        user.PasswordHash = newHash;
        await _context.SaveChangesAsync();
        
        return Ok();
    }

    private string GetUserLevel(int orders)
    {
        if (orders > 20) return "Гуру";
        if (orders > 5) return "Книголюб";
        return "Читач";

    }
}