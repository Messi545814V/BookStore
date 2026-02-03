using BookStore.Core.DTOs;
using BookStore.Core.Entities;

namespace BookStore.Core.Interfaces;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(Order order);
    // Task RemoveFromCartAsync(int id);
    Task<int> PlaceOrderAsync(string userId);
    Task<IEnumerable<Order>> GetUserOrdersAsync(int userId);
    
    Task<IEnumerable<OrderSummaryDto>> GetAllOrdersAsync();
    Task<int> PlaceOrderWithDetailsAsync(string userIdString, CheckoutOrderDto dto);
    
    Task<Order?> GetOrderByIdAsync(int orderId);
    
    Task UpdateOrderStatusAsync(int orderId, string newStatus);

}
