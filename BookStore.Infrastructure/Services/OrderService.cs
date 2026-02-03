using BookStore.Core.DTOs;
using BookStore.Core.Entities;
using BookStore.Core.Interfaces;
using BookStore.Infrastructure.Data; 
using BookStore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore; 

namespace BookStore.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly BookStoreContext _context;
    private readonly BonusService _bonusService;
  //  private IOrderService _orderServiceImplementation;

    public OrderService(BookStoreContext context,  BonusService bonusService)
    {
        _context = context;
        _bonusService = bonusService;
    }
    
    public async Task<Order> CreateOrderAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
        return order;
    }
    
    public async Task<IEnumerable<Order>> GetUserOrdersAsync(int userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Book)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    // 👇 ОСЬ ПОВНІСТЮ ВИПРАВЛЕНИЙ МЕТОД 👇
    
    public async Task<int> PlaceOrderAsync(string userIdString)
    {
        if (!int.TryParse(userIdString, out var userId))
        {
            throw new ArgumentException("Invalid user ID format", nameof(userIdString));
        }
        
        var cartItems = await _context.CartItems
            .Where(ci => ci.UserId == userId)
            .Include(ci => ci.Book)
            .ToListAsync();

        if (!cartItems.Any())
        {
            throw new InvalidOperationException("Cannot place an order with an empty cart.");
        }
        
        var order = new Order
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            Amount = cartItems.Sum(item => item.Quantity * item.Book.Price)
        };
        
        await _context.Orders.AddAsync(order);
        _context.CartItems.RemoveRange(cartItems);
        
        await _context.SaveChangesAsync();
        
        return order.Id;
    }

    public async Task<IEnumerable<OrderSummaryDto>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.User)
            .Select(o => new OrderSummaryDto
            {
                Id = o.Id,
                CreatedAt = o.CreatedAt,
                UserId = o.UserId,
                UserName = o.User.Username,
                Amount = o.Amount,
                Status = o.Status,
                FirstName = o.FirstName,
                LastName = o.LastName,
                Phone = o.Phone,
                City = o.City,
                DeliveryMethod = o.DeliveryMethod,
                PaymentMethod = o.PaymentMethod,
                Warehouse = o.Warehouse,
                Address = o.Address,
                Comment = o.Comment,
            })
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> PlaceOrderWithDetailsAsync(string userIdString, CheckoutOrderDto dto)
    {
        if (!int.TryParse(userIdString, out var userId))
            throw new ArgumentException("Invalid user ID");

        var cartItems = await _context.CartItems
            .Where(ci => ci.UserId == userId)
            .Include(ci => ci.Book)
            .ToListAsync();

        if (!cartItems.Any())
            throw new InvalidOperationException("Cart is empty");
        
        decimal itemsTotal = cartItems.Sum(item => item.Quantity * item.Book.Price);

        decimal discount = 0;
        if (dto.BonusesToUse > 0)
        {
            
        }

        var order = new Order
        {
            UserId = userId,
            Amount = cartItems.Sum(i => i.Book.Price * i.Quantity),

            // Контакти
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Phone = dto.Phone,
            Email = dto.Email,

            // Доставка
            DeliveryMethod = dto.DeliveryMethod,
            City = dto.City,
            CityRef = dto.CityRef,
            Warehouse = dto.Warehouse,

            // Кур’єр
            Address = dto.Address,
            House = dto.House,
            Apartment = dto.Apartment,
            
            DeliveryStatus = "New",
            
            PaymentMethod = dto.PaymentMethod,
            
            Comment = dto.Comment,

            CreatedAt = DateTime.UtcNow
        };

        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        // Додаємо OrderItems
        foreach (var item in cartItems)
        {
            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                BookId = item.BookId,
                Quantity = item.Quantity,
                UnitPrice = item.Book.Price
            };

            await _context.OrderItems.AddAsync(orderItem);
        }
        
        if (dto.BonusesToUse > 0)
        {
            bool success = await _bonusService.UseBonusesAsync(userId, dto.BonusesToUse, order.Id);
            if (!success) 
            {
                // Якщо раптом бонусів не вистачило (хакери?), можна відкотити замовлення
                // throw new Exception("Error using bonuses");
            }
        }

        // Очищаємо кошик
        _context.CartItems.RemoveRange(cartItems);

        await _context.SaveChangesAsync();

        return order.Id;
    }

    // BookStore.Infrastructure/Services/OrderService.cs
    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Book) // Важливо: підвантажуємо книги для кошика
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task UpdateOrderStatusAsync(int orderId, string newStatus)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
          

            if (newStatus == "Shipped") order.DeliveryStatus = "Sent"; 
            if (newStatus == "Completed") order.DeliveryStatus = "Delivered";
            
            
            if (newStatus == "Completed" && order.Status != "Completed")
            {
                // Нараховуємо бонуси за суму, яку він реально сплатив грошима
                await _bonusService.AccrueBonusesAsync(order.UserId, order.Id, order.Amount);
            }
            
            order.Status = newStatus;
            await _context.SaveChangesAsync();
        }
        
        
    }


}