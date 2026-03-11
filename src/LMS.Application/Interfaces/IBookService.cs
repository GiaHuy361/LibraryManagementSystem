using LMS.Application.Common;
using LMS.Application.DTOs.Books;

namespace LMS.Application.Interfaces;

/// <summary>UC-07 Add Book / UC-08 Update Book / UC-09 Delete Book / UC-16 Search / UC-17 View Details</summary>
public interface IBookService
{
    Task<Result<PagedResult<BookDto>>> GetAllBooksAsync(PaginationParams pagination);
    Task<Result<BookDto>> GetBookByIdAsync(int bookId);
    Task<Result<PagedResult<BookDto>>> SearchBooksAsync(BookSearchDto dto);
    Task<Result<BookDto>> CreateBookAsync(CreateBookDto dto);
    Task<Result<BookDto>> UpdateBookAsync(int bookId, UpdateBookDto dto);
    Task<Result> DeleteBookAsync(int bookId);
}
