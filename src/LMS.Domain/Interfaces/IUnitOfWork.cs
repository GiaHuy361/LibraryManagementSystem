namespace LMS.Domain.Interfaces;

/// <summary>
/// Unit of Work pattern: coordinates transactions across multiple repositories.
/// Ensures all changes within a single operation are committed atomically.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IBookRepository Books { get; }
    IUserRepository Users { get; }
    IBorrowRepository Borrows { get; }
    IFineRepository Fines { get; }
    IRepository<LMS.Domain.Entities.Role> Roles { get; }
    IRepository<LMS.Domain.Entities.Category> Categories { get; }
    IRepository<LMS.Domain.Entities.Author> Authors { get; }
    IRepository<LMS.Domain.Entities.BorrowDetail> BorrowDetails { get; }

    Task<int> SaveChangesAsync();
}
