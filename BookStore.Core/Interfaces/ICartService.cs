// BookStore.Core/Interfaces/ICartService.cs
using BookStore.Core.DTOs;

namespace BookStore.Core.Interfaces;

public interface ICartService
{
    Task AddToCartAsync(string userId, int bookId, int quantity);

    Task RemoveFromCartAsync(string userId, int bookId);
    
    Task<List<CartItemDto>> GetUserCartAsync(string userId);
    Task UpdateQuantityAsync(string userId, int bookId, int quantity);
}