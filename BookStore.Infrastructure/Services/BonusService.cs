using BookStore.Core.Entities;
using BookStore.Infrastructure.Data;

namespace BookStore.Infrastructure.Services;

public class BonusService
{
    private readonly BookStoreContext _context;
    
    public BonusService(BookStoreContext context)
    {
        _context = context;
    }

    public async Task AccrueBonusesAsync(int userId, int orderId, decimal orderTotal)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return;
        
        user.TotalSpent += orderTotal;
        
        decimal cashbackPercent = GetCashbackPercent(user.TotalSpent);
        
        decimal bonusAmount = Math.Round(orderTotal * cashbackPercent);

        if (bonusAmount > 0)
        {
            user.BonusBalance += bonusAmount;
            
            _context.BonusTransactions.Add(new BonusTransaction
            {
                UserId = userId,
                Amount = bonusAmount,
                OrderId = orderId,
                Description = $"Кешбек за замовлення #{orderId}",
                Date = DateTime.UtcNow
            });
            
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> UseBonusesAsync(int userId, decimal amountToUse, int orderId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || user.BonusBalance < amountToUse) return false;
        
        user.BonusBalance -= amountToUse;
        
        _context.BonusTransactions.Add(new BonusTransaction
        {
            UserId = userId,
            Amount = -amountToUse, // Мінус
            OrderId = orderId,
            Description = $"Оплата частини замовлення #{orderId}",
            Date = DateTime.UtcNow
        });
        
        await _context.SaveChangesAsync();
        return true;
    }

    public decimal GetCashbackPercent(decimal totalSpent)
    {
        if (totalSpent >= 15000) return 0.10m;
        if (totalSpent >= 25000) return 0.05m;
        return 0.03m;
    }

    public string GetUserLevelName(decimal totalSpent)
    {
        if (totalSpent >= 15000) return "Експерт";
        if (totalSpent >= 5000) return "Книгоман";
        return "Читач";
    }
}