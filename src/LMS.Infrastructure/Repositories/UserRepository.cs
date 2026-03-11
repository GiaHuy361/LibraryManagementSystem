using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(LibraryDbContext context) : base(context) { }

    public async Task<User?> GetByUsernameAsync(string username)
        => await _dbSet.Include(u => u.Role).FirstOrDefaultAsync(u => u.Username == username);

    public async Task<User?> GetByEmailAsync(string email)
        => await _dbSet.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetWithRoleAsync(int userId)
        => await _dbSet.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId);

    public async Task<IEnumerable<User>> GetByRoleAsync(int roleId)
        => await _dbSet.Include(u => u.Role).Where(u => u.RoleId == roleId).ToListAsync();
}
