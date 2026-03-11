using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;

namespace LMS.Infrastructure.Repositories;

/// <summary>
/// Unit of Work: wraps all repositories under a single DbContext transaction.
/// Calling SaveChangesAsync() commits all pending changes atomically.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly LibraryDbContext _context;

    private IBookRepository? _books;
    private IUserRepository? _users;
    private IBorrowRepository? _borrows;
    private IFineRepository? _fines;
    private IRepository<Role>? _roles;
    private IRepository<Category>? _categories;
    private IRepository<Author>? _authors;
    private IRepository<BorrowDetail>? _borrowDetails;

    public UnitOfWork(LibraryDbContext context) => _context = context;

    // Lazy initialization — repositories are only created when first accessed
    public IBookRepository Books => _books ??= new BookRepository(_context);
    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IBorrowRepository Borrows => _borrows ??= new BorrowRepository(_context);
    public IFineRepository Fines => _fines ??= new FineRepository(_context);
    public IRepository<Role> Roles => _roles ??= new GenericRepository<Role>(_context);
    public IRepository<Category> Categories => _categories ??= new GenericRepository<Category>(_context);
    public IRepository<Author> Authors => _authors ??= new GenericRepository<Author>(_context);
    public IRepository<BorrowDetail> BorrowDetails => _borrowDetails ??= new GenericRepository<BorrowDetail>(_context);

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
