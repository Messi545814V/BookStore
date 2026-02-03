using System.Security.Claims;
using BookStore.Core.DTOs;
using BookStore.Core.Interfaces; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }


    [HttpGet]
    public async Task<ActionResult<List<CartItemDto>>> GetUserCart()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }


        var items = await _cartService.GetUserCartAsync(userId); 
        return Ok(items);
    }


    [HttpPost("add")]
    public async Task<IActionResult> AddToCart(CartItemDto itemToAdd) 
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }


        await _cartService.AddToCartAsync(userId, itemToAdd.BookId, itemToAdd.Quantity);
        return Ok();
    }


    [HttpDelete("{bookId:int}")]
    public async Task<IActionResult> RemoveFromCart(int bookId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        await _cartService.RemoveFromCartAsync(userId, bookId);
        return Ok();
    }
    
    [HttpPost("update")]
    public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityDto model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        await _cartService.UpdateQuantityAsync(userId, model.BookId, model.Delta);

        return Ok();
    }


}