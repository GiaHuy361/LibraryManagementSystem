using LMS.Domain.Entities;

namespace LMS.Domain.Interfaces;

/// <summary>
/// Specialized book repository with search and availability queries.
/// </summary>
public interface IBookRepository : IRepository<Book>
{
    Task<IEnumerable<Book>> SearchAsync(string? title, string? author, int? categoryId);
    Task<Book?> GetByIsbnAsync(string isbn);
    Task<Book?> GetWithDetailsAsync(int bookId);
}
