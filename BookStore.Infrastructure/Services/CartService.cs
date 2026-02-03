using BookStore.Core.DTOs;
using BookStore.Core.Entities;
using BookStore.Core.Interfaces;
using BookStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly BookStoreContext _context;

    public CartService(BookStoreContext context)
    {
        _context = context;
    }


    public async Task<List<CartItemDto>> GetUserCartAsync(string userId)
    {

        if (!int.TryParse(userId, out var parsedUserId))
        {
            return new List<CartItemDto>(); 
        }

        var items = await _context.CartItems
            .Where(ci => ci.UserId == parsedUserId) 
            .Include(ci => ci.Book)
            .Select(ci => new CartItemDto
            {
                BookId = ci.BookId,
                BookTitle = ci.Book.Title, 
                Price = ci.Book.Price,
                Quantity = ci.Quantity,
                ImageUrl = ci.Book.ImageUrl
            })
            .ToListAsync();

        return items;
    }
    
    public async Task AddToCartAsync(string userId, int bookId, int quantity)
    {
        if (!int.TryParse(userId, out var parsedUserId)) return;

        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.UserId == parsedUserId && ci.BookId == bookId);

        if (cartItem != null)
        {
            cartItem.Quantity += quantity;
        }
        else
        {
            cartItem = new CartItem
            {
                UserId = parsedUserId,
                BookId = bookId,
                Quantity = quantity,
            };
            await _context.CartItems.AddAsync(cartItem);
        }

        await _context.SaveChangesAsync();
    }

    // 👇 ВИПРАВЛЕННЯ: Змінили 'int' на 'string'
    public async Task RemoveFromCartAsync(string userId, int bookId)
    {
        if (!int.TryParse(userId, out var parsedUserId)) return;

        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.UserId == parsedUserId && ci.BookId == bookId);

        if (cartItem != null)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task UpdateQuantityAsync(string userId, int bookId, int delta)
    {
        if (!int.TryParse(userId, out var parsedUserId)) return;

        var item = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.UserId == parsedUserId && ci.BookId == bookId);

        if (item == null) return;

        item.Quantity += delta;

        if (item.Quantity <= 0)
        {
            _context.CartItems.Remove(item);
        }

        await _context.SaveChangesAsync();
    }

}