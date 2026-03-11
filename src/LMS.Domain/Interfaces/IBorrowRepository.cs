using LMS.Domain.Entities;

namespace LMS.Domain.Interfaces;

/// <summary>
/// Specialized borrow repository with filtered queries for librarian views.
/// </summary>
public interface IBorrowRepository : IRepository<BorrowRecord>
{
    Task<BorrowRecord?> GetWithDetailsAsync(int borrowId);
    Task<IEnumerable<BorrowRecord>> GetByMemberAsync(int memberId);
    Task<IEnumerable<BorrowRecord>> GetByDateRangeAsync(DateOnly from, DateOnly to);
    Task<IEnumerable<BorrowRecord>> GetActiveRecordsAsync();
    Task<int> CountTotalBorrowsAsync();
}
