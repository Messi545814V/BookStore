using BookStore.Core.Entities;
using BookStore.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorController : ControllerBase
{
    private readonly AuthorService _authorService;

    public AuthorController(AuthorService authorService)
    {
        _authorService = authorService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
    {
        var authors = await _authorService.GetAuthors();
        return Ok(authors);
    }

    [HttpGet("{authorId}")]
    public async Task<ActionResult<Author>> GetAuthor(int authorId)
    {
        var author = await _authorService.GetAuthorById(authorId);
        if (author == null) return NotFound();
        return Ok(author);
    }

    [HttpPost]
    public async Task<ActionResult<Author>> AddAuthor(Author author)
    {
        var newAuthor = await _authorService.AddAuthor(author);
        return CreatedAtAction(nameof(GetAuthor), new { authorId = newAuthor.Id }, newAuthor);
    }

    [HttpPut("{authorId}")]
    public async Task<ActionResult<Author>> UpdateAuthor(int authorId, Author author)
    {
        if (authorId != author.Id) return BadRequest();
        
        var updated = await _authorService.UpdateAuthor(author);

        return Ok(updated);
    }

    [HttpDelete("{authorId}")]
    public async Task<ActionResult<Author>> DeleteAuthor(int authorId)
    {
        await _authorService.DeleteAuthor(authorId);
        return NoContent();
    }
}