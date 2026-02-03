using BookStore.Core.DTOs;
using BookStore.Core.Entities;
using BookStore.Core.Interfaces;
using Microsoft.AspNetCore.Authorization; // <-- ВАЖЛИВО: додай цей using
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // 👇👇👇 ОСЬ ТУТ КЛЮЧОВЕ ВИПРАВЛЕННЯ 👇👇👇
    public class BooksController(IBookService bookService) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PageResult<BookSummaryDto>>> GetBooks([FromQuery] BookFilterDto filter)
        {
            Console.WriteLine($"🔥 SearchTerm from API = '{filter.SearchTerm}'");
            return Ok(await bookService.GetAllBooksAsync(filter));
        }


        [HttpGet("categories")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Category>>> GetCategories()
        {
            var categories = await bookService.GetCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<BookDetailsDto>> GetBook(int id)
        {
            var book = await bookService.GetBookByIdWithReviewsAsync(id); // 👈 НОВИЙ метод
    
            if (book == null)
                return NotFound();

            return Ok(book);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Book>> PostBook([FromBody] BookSaveDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var book = new Book
            {
                Title = dto.Title,
                Price = dto.Price,
                Description = dto.Description, 
                Language = dto.Language,     
                Year = dto.Year,
                Stock = dto.Stock,
                Genre = dto.Genre,
                AuthorId = dto.AuthorId,
                CategoryId = dto.CategoryId,
                ImageUrl = dto.ImageUrl
            };

            Console.WriteLine($"📌 BOOK ENTITY GENRE = '{book.Genre}'");
            
            await bookService.AddBookAsync(book);

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Book>> UpdateBook(int id, [FromBody] BookSaveDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.Id)
            {
                return BadRequest();
            }
            

            var book = new Book
            {
                Id = dto.Id,
                Title = dto.Title,
                Price = dto.Price,
                Year = dto.Year,
                Stock = dto.Stock,
                Genre = dto.Genre,
                AuthorId = dto.AuthorId,
                CategoryId = dto.CategoryId,
                ImageUrl = dto.ImageUrl,
                Language = dto.Language,
                Description = dto.Description,
            };
            
            await bookService.UpdateBookAsync(book);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Book>> DeleteBook(int id)
        {
            await bookService.DeleteBookAsync(id);
            return NoContent();
        }

        [HttpGet("tree-used")]
        public async Task<ActionResult<List<CategoryTreeDto>>> GetUsedCategoriesTree()
        {
            return Ok(await bookService.GetUsedCategoryTreeAsync());
        }

    }
}