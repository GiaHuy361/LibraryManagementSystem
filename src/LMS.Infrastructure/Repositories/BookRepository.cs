using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class BookRepository : GenericRepository<Book>, IBookRepository
{
    public BookRepository(LibraryDbContext context) : base(context) { }

    public async Task<IEnumerable<Book>> SearchAsync(string? title, string? author, int? categoryId)
    {
        var query = _context.Books
            .Include(b => b.Category)
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(b => b.Title.Contains(title));

        if (!string.IsNullOrWhiteSpace(author))
            query = query.Where(b => b.BookAuthors.Any(ba => ba.Author!.AuthorName.Contains(author)));

        if (categoryId.HasValue)
            query = query.Where(b => b.CategoryId == categoryId.Value);

        return await query.ToListAsync();
    }

    public async Task<Book?> GetByIsbnAsync(string isbn)
        => await _dbSet.FirstOrDefaultAsync(b => b.Isbn == isbn);

    public async Task<Book?> GetWithDetailsAsync(int bookId)
        => await _context.Books
            .Include(b => b.Category)
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .FirstOrDefaultAsync(b => b.BookId == bookId);
}
