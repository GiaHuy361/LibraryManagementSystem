using LMS.Application.Common;
using LMS.Application.DTOs.Books;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    public BooksController(IBookService bookService) => _bookService = bookService;

    /// <summary>UC-16: Search books by title, author, or category. Open to all authenticated users.</summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] BookSearchDto dto)
    {
        var result = await _bookService.SearchBooksAsync(dto);
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    /// <summary>Get all books with pagination.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination)
    {
        var result = await _bookService.GetAllBooksAsync(pagination);
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    /// <summary>UC-17: View book details including title, author, category, status, description.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _bookService.GetBookByIdAsync(id);
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    /// <summary>UC-07: Admin adds a new book.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateBookDto dto)
    {
        var result = await _bookService.CreateBookAsync(dto);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.BookId }, result.Data)
            : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    /// <summary>UC-08: Admin updates book info.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBookDto dto)
    {
        var result = await _bookService.UpdateBookAsync(id, dto);
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    /// <summary>UC-09: Admin deletes a book.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _bookService.DeleteBookAsync(id);
        return result.IsSuccess ? NoContent() : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }
}
