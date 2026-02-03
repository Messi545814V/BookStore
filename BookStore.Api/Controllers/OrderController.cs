using System.Security.Claims;
using BookStore.Core.DTOs;
using BookStore.Core.Entities;
using BookStore.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<int>> PlaceOrder()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        
        var orderId = await _orderService.PlaceOrderAsync(userId);
        return Ok(orderId);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateOrderAsync(Order order)
    {
        var newOrder = await _orderService.CreateOrderAsync(order);
        return Ok(newOrder);    
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyOrders()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId)) return Unauthorized();
        
        var orders = await _orderService.GetUserOrdersAsync(userId);
        
        var dtos = orders.Select(o => new OrderDto
        {
            Id = o.Id,
            OrderDate = o.CreatedAt,
            TotalPrice = o.Amount,
            Status = o.Status,
            Items = o.OrderItems.Select(oi => new OrderItemDto
            {
                BookId = oi.BookId,
                Title = oi.Book.Title,
                ImageUrl = oi.Book.ImageUrl,
                Quantity = oi.Quantity,
                Price = oi.UnitPrice
            }).ToList()
        });

        return Ok(dtos);
    }
    
    [HttpGet("user/{userId}")] 
    public async Task<IActionResult> GetOrders(int userId)
    {
        var orders = await _orderService.GetUserOrdersAsync(userId);
        return Ok(orders);
    }

    [HttpGet("all")] 
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<OrderSummaryDto>>> GetAllOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    [HttpPost("checkout")]
    public async Task<ActionResult<int>> Checkout([FromBody] CheckoutOrderDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var orderId = await _orderService.PlaceOrderWithDetailsAsync(userId, dto);
        return Ok(orderId);
    }


    [HttpGet("{orderId:int}")]
    public async Task<IActionResult> GetOrderById(int orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);

        if (order == null) return NotFound("Замовлення не знайдено");
        
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (!int.TryParse(userIdString, out int currentUserId)) return Unauthorized();
        
        if (order.UserId != currentUserId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        return Ok(order);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string newStatus)
    {
        await _orderService.UpdateOrderStatusAsync(id, newStatus);
        return Ok();
    }
}