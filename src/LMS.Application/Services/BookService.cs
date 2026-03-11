using LMS.Application.Common;
using LMS.Application.DTOs.Books;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;

namespace LMS.Application.Services;

/// <summary>
/// Handles UC-07 Add Book, UC-08 Update Book, UC-09 Delete Book, UC-16 Search, UC-17 View Details.
/// </summary>
public class BookService : IBookService
{
    private readonly IUnitOfWork _uow;

    public BookService(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<PagedResult<BookDto>>> GetAllBooksAsync(PaginationParams pagination)
    {
        var books = await _uow.Books.GetAllAsync();
        var totalCount = books.Count();
        
        var pagedBooks = books
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(MapToDto)
            .ToList();

        var pagedResult = new PagedResult<BookDto>(pagedBooks, totalCount, pagination.PageNumber, pagination.PageSize);
        return Result<PagedResult<BookDto>>.Success(pagedResult);
    }

    public async Task<Result<BookDto>> GetBookByIdAsync(int bookId)
    {
        var book = await _uow.Books.GetWithDetailsAsync(bookId);
        if (book == null) return Result<BookDto>.NotFound($"Book {bookId} not found.");
        return Result<BookDto>.Success(MapToDto(book));
    }

    public async Task<Result<PagedResult<BookDto>>> SearchBooksAsync(BookSearchDto dto)
    {
        var books = await _uow.Books.SearchAsync(dto.Title, dto.Author, dto.CategoryId);
        var totalCount = books.Count();

        var pagedBooks = books
            .Skip((dto.PageNumber - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .Select(MapToDto)
            .ToList();

        var pagedResult = new PagedResult<BookDto>(pagedBooks, totalCount, dto.PageNumber, dto.PageSize);
        return Result<PagedResult<BookDto>>.Success(pagedResult);
    }

    public async Task<Result<BookDto>> CreateBookAsync(CreateBookDto dto)
    {
        // Check ISBN uniqueness
        if (!string.IsNullOrWhiteSpace(dto.Isbn))
        {
            var existing = await _uow.Books.GetByIsbnAsync(dto.Isbn);
            if (existing != null) return Result<BookDto>.Conflict($"ISBN '{dto.Isbn}' already exists.");
        }

        var book = BookFactory.Create(dto);
        await _uow.Books.AddAsync(book);
        await _uow.SaveChangesAsync();

        // Attach authors via BookAuthor join
        foreach (var authorId in dto.AuthorIds)
        {
            var author = await _uow.Authors.GetByIdAsync(authorId);
            if (author != null)
                book.BookAuthors.Add(new BookAuthor { BookId = book.BookId, AuthorId = authorId });
        }
        if (dto.AuthorIds.Any())
        {
            _uow.Books.Update(book);
            await _uow.SaveChangesAsync();
        }

        var created = await _uow.Books.GetWithDetailsAsync(book.BookId);
        return Result<BookDto>.Created(MapToDto(created!));
    }

    public async Task<Result<BookDto>> UpdateBookAsync(int bookId, UpdateBookDto dto)
    {
        var book = await _uow.Books.GetWithDetailsAsync(bookId);
        if (book == null) return Result<BookDto>.NotFound($"Book {bookId} not found.");

        if (dto.Title != null) book.Title = dto.Title;
        if (dto.Isbn != null) book.Isbn = dto.Isbn;
        if (dto.CategoryId.HasValue) book.CategoryId = dto.CategoryId;
        if (dto.Publisher != null) book.Publisher = dto.Publisher;
        if (dto.PublishYear.HasValue) book.PublishYear = dto.PublishYear;
        if (dto.Description != null) book.Description = dto.Description;
        if (dto.Quantity.HasValue) book.Quantity = dto.Quantity.Value;
        if (dto.Status != null) book.Status = dto.Status;

        if (dto.AuthorIds != null)
        {
            book.BookAuthors.Clear();
            foreach (var authorId in dto.AuthorIds)
                book.BookAuthors.Add(new BookAuthor { BookId = bookId, AuthorId = authorId });
        }

        _uow.Books.Update(book);
        await _uow.SaveChangesAsync();
        return Result<BookDto>.Success(MapToDto(book));
    }

    public async Task<Result> DeleteBookAsync(int bookId)
    {
        var book = await _uow.Books.GetByIdAsync(bookId);
        if (book == null) return Result.NotFound($"Book {bookId} not found.");

        _uow.Books.Remove(book);
        await _uow.SaveChangesAsync();
        return Result.Success();
    }

    private static BookDto MapToDto(Book b) => new()
    {
        BookId = b.BookId,
        Title = b.Title,
        Isbn = b.Isbn,
        CategoryId = b.CategoryId,
        CategoryName = b.Category?.CategoryName,
        Publisher = b.Publisher,
        PublishYear = b.PublishYear,
        Description = b.Description,
        Quantity = b.Quantity,
        Status = b.Status,
        Authors = b.BookAuthors.Select(ba => ba.Author?.AuthorName ?? string.Empty).ToList()
    };
}

/// <summary>Factory Pattern: centralizes Book entity creation.</summary>
public static class BookFactory
{
    public static Book Create(CreateBookDto dto) => new()
    {
        Title = dto.Title,
        Isbn = dto.Isbn,
        CategoryId = dto.CategoryId,
        Publisher = dto.Publisher,
        PublishYear = dto.PublishYear,
        Description = dto.Description,
        Quantity = dto.Quantity,
        Status = "Available",
        CreatedAt = DateTime.UtcNow
    };
}
