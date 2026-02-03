using BookStore.Infrastructure.Data;
using BookStore.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly LiqPayService _liqPayService;
    private readonly BookStoreContext _context;
    
    public PaymentController(LiqPayService liqPayService, BookStoreContext context)
    {
        _liqPayService = liqPayService;
        _context = context;
    }

    [HttpGet("get-params/{orderid}")]
    public async Task<IActionResult> GetLiqPayParams(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null) return NotFound("Замовлення не знайдено");

        var dto =  _liqPayService.GetLiqPayRequest(order.Id, order.Amount);
        return Ok(dto);
    }
    
    [HttpGet("check-status/{orderId}")]
    public async Task<IActionResult> CheckPaymentStatus(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null) return NotFound();

        if (order.PaymentMethod != "Онлайн")
        {
            return Ok(new {Status = "success"});
        }
        
        var status = await _liqPayService.CheckStatus(order.Id.ToString());

        if (status == "success" || status == "sandbox")
        {
            order.PaymentMethod = "Paid(Online)";
            
            await _context.SaveChangesAsync();
        }
        
        return Ok(new{Status = status});
    }
    
}