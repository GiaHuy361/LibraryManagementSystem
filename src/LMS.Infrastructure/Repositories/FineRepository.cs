using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class FineRepository : GenericRepository<Fine>, IFineRepository
{
    public FineRepository(LibraryDbContext context) : base(context) { }

    public async Task<Fine?> GetByBorrowDetailAsync(int borrowDetailId)
        => await _context.Fines
            .Include(f => f.BorrowDetail).ThenInclude(d => d!.Book)
            .FirstOrDefaultAsync(f => f.BorrowDetailId == borrowDetailId);

    public async Task<IEnumerable<Fine>> GetUnpaidFinesAsync()
        => await _context.Fines
            .Include(f => f.BorrowDetail).ThenInclude(d => d!.Book)
            .Where(f => !f.PaidStatus)
            .ToListAsync();

    public async Task<decimal> GetTotalFinesCollectedAsync()
        => await _context.Fines
            .Where(f => f.PaidStatus)
            .SumAsync(f => f.FineAmount);
}
