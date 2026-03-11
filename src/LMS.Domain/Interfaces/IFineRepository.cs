using LMS.Domain.Entities;

namespace LMS.Domain.Interfaces;

/// <summary>
/// Specialized fine repository with payment status queries.
/// </summary>
public interface IFineRepository : IRepository<Fine>
{
    Task<Fine?> GetByBorrowDetailAsync(int borrowDetailId);
    Task<IEnumerable<Fine>> GetUnpaidFinesAsync();
    Task<decimal> GetTotalFinesCollectedAsync();
}
