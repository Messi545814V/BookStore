using System.Security.Claims;
using BookStore.Core.DTOs;
using BookStore.Core.Entities;
using BookStore.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Api.Controllers;

[ApiController]
[Route("api/books/{bookId}/comments")]
public class CommentsController : ControllerBase
{
    private readonly BookStoreContext _context;
    
    public CommentsController(BookStoreContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<BookCommentDto>>> GetComments(int bookId)
    {
        int currentUserId = 0;
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim != null) int.TryParse(userIdClaim, out currentUserId);

        bool isAdmin = User.IsInRole("Admin");
        
        var allComments = await _context.BookComments
            .Where(c => c.BookId == bookId && (c.IsApproved == true || c.UserId == currentUserId || isAdmin))
            .Include(c => c.User)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
        
        var adminUserIds = new List<int>();
        try 
        {
            var adminRole = await _context.Set<IdentityRole<int>>()
                .FirstOrDefaultAsync(r => r.Name == "Admin");

            if (adminRole != null)
            {
                adminUserIds = await _context.Set<IdentityUserRole<int>>()
                    .Where(ur => ur.RoleId == adminRole.Id)
                    .Select(ur => ur.UserId)
                    .ToListAsync();
            }
        }
        catch (Exception)
        {
            Console.WriteLine("Warning: Could not fetch admin roles.");
        }
        
        var dtos = allComments.Select(c => new BookCommentDto
        {
            Id = c.Id,
            UserId = c.UserId,
            UserName = c.User?.Username ?? "Unknown",
            Comment = c.Comment,
            CreatedAt = c.CreatedAt,
            IsApproved = c.IsApproved,
            ParentId = c.ParentId,
       
            IsAdmin = adminUserIds.Contains(c.UserId) 
        }).ToList();
        
        var rootComments = new List<BookCommentDto>();
        var lookup = dtos.ToDictionary(c => c.Id);

        foreach (var c in dtos)
        {
            if (c.ParentId.HasValue && lookup.ContainsKey(c.ParentId.Value))
                lookup[c.ParentId.Value].Replies.Add(c);
            else
                rootComments.Add(c);
        }

        var finalResult = rootComments
            .Where(r => r.ParentId == null || !lookup.ContainsKey(r.ParentId.Value)) 
            .OrderByDescending(c => c.CreatedAt)
            .ToList();

        return Ok(finalResult);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddComment(int bookId, [FromBody] CreateCommentDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Comment))
            return BadRequest("Comment cannot be empty");

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        bool isUserAdmin = User.IsInRole("Admin");

        var comment = new BookComment
        {
            BookId = bookId,
            UserId = userId,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow,
            ParentId = dto.ParentId,

            IsApproved = isUserAdmin 
        };

        _context.BookComments.Add(comment);
        await _context.SaveChangesAsync();
        return Ok();
    }
    
    [Authorize]
    [HttpPut("{commentId:int}")]
    public async Task<IActionResult> UpdateComment(int bookId, int commentId, [FromBody] CreateCommentDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Comment)) return BadRequest();
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        
        var comment = await _context.BookComments.FirstOrDefaultAsync(c => c.Id == commentId && c.BookId == bookId);
        if (comment == null) return NotFound();
        if (comment.UserId != userId) return Forbid();

        comment.Comment = dto.Comment;
        await _context.SaveChangesAsync();
        return Ok();
    }

    [Authorize]
    [HttpDelete("{commentId:int}")]
    public async Task<IActionResult> DeleteComment(int bookId, int commentId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        
        var comment = await _context.BookComments.FirstOrDefaultAsync(c => c.Id == commentId && c.BookId == bookId);
        if (comment == null) return NotFound();
        
        if (comment.UserId != userId && !User.IsInRole("Admin")) return Forbid();

        _context.BookComments.Remove(comment);
        await _context.SaveChangesAsync();
        return Ok();
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("/api/admin/comments")]
    public async Task<ActionResult<List<AdminCommentDto>>> GetAllComments()
    {
        var comments = await _context.BookComments
            .Include(c => c.Book)
            .Include(c => c.User)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new AdminCommentDto
            {
                Id = c.Id,
                BookId = c.BookId,
                BookTitle = c.Book.Title,
                UserName = c.User.Username,
                Comment = c.Comment,
                CreatedAt = c.CreatedAt,
                IsApproved = c.IsApproved
            })
            .ToListAsync();

        return Ok(comments);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("/api/admin/comments/{commentId}/approve")]
    public async Task<IActionResult> ApproveComment(int commentId)
    {
        var comment = await _context.BookComments.FindAsync(commentId);
        if (comment == null) return NotFound();
        comment.IsApproved = true;
        await _context.SaveChangesAsync();
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("/api/admin/comments/{commentId}")]
    public async Task<IActionResult> DeleteAdminComment(int commentId)
    {
        var comment = await _context.BookComments.FindAsync(commentId);
        if (comment == null) return NotFound();
        _context.BookComments.Remove(comment);
        await _context.SaveChangesAsync();
        return Ok();
    }
}