using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class BorrowRepository : GenericRepository<BorrowRecord>, IBorrowRepository
{
    public BorrowRepository(LibraryDbContext context) : base(context) { }

    public async Task<BorrowRecord?> GetWithDetailsAsync(int borrowId)
        => await _context.BorrowRecords
            .Include(r => r.Member)
            .Include(r => r.Librarian)
            .Include(r => r.BorrowDetails)
                .ThenInclude(d => d.Book)
            .FirstOrDefaultAsync(r => r.BorrowId == borrowId);

    public async Task<IEnumerable<BorrowRecord>> GetByMemberAsync(int memberId)
        => await _context.BorrowRecords
            .Include(r => r.Member)
            .Include(r => r.BorrowDetails).ThenInclude(d => d.Book)
            .Where(r => r.MemberId == memberId)
            .OrderByDescending(r => r.BorrowDate)
            .ToListAsync();

    public async Task<IEnumerable<BorrowRecord>> GetByDateRangeAsync(DateOnly from, DateOnly to)
        => await _context.BorrowRecords
            .Include(r => r.Member)
            .Include(r => r.BorrowDetails).ThenInclude(d => d.Book)
            .Where(r => r.BorrowDate >= from && r.BorrowDate <= to)
            .OrderByDescending(r => r.BorrowDate)
            .ToListAsync();

    public async Task<IEnumerable<BorrowRecord>> GetActiveRecordsAsync()
        => await _context.BorrowRecords
            .Include(r => r.Member)
            .Include(r => r.BorrowDetails).ThenInclude(d => d.Book)
            .Where(r => r.Status == "Borrowing")
            .ToListAsync();

    public async Task<int> CountTotalBorrowsAsync()
        => await _dbSet.CountAsync();
}
