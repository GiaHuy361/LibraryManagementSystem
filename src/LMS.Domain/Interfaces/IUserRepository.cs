using LMS.Domain.Entities;

namespace LMS.Domain.Interfaces;

/// <summary>
/// Specialized user repository with username/email lookup.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetWithRoleAsync(int userId);
    Task<IEnumerable<User>> GetByRoleAsync(int roleId);
}
