using System.Linq.Expressions;

namespace LMS.Domain.Interfaces;

/// <summary>
/// Generic repository interface providing standard CRUD operations.
/// Follows Interface Segregation – only expose what each consumer needs.
/// </summary>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}
