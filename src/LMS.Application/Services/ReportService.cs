using LMS.Application.Common;
using LMS.Application.DTOs.Fines;
using LMS.Application.Interfaces;
using LMS.Domain.Interfaces;

namespace LMS.Application.Services;

/// <summary>Handles UC-11: Admin views summary statistics report.</summary>
public class ReportService : IReportService
{
    private readonly IUnitOfWork _uow;
    public ReportService(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<ReportDto>> GetSummaryReportAsync()
    {
        var books = await _uow.Books.GetAllAsync();
        var totalBorrows = await _uow.Borrows.CountTotalBorrowsAsync();
        var activeRecords = await _uow.Borrows.GetActiveRecordsAsync();
        var totalFinesCollected = await _uow.Fines.GetTotalFinesCollectedAsync();
        var unpaidFines = await _uow.Fines.GetUnpaidFinesAsync();

        var activeList = activeRecords.ToList();
        var overdueCount = activeList.Count(r => r.IsOverdue());

        var report = new ReportDto
        {
            TotalBooks = books.Count(),
            TotalBorrows = totalBorrows,
            ActiveBorrows = activeList.Count,
            OverdueRecords = overdueCount,
            TotalFinesCollected = totalFinesCollected,
            TotalFinesPending = unpaidFines.Sum(f => f.FineAmount)
        };

        return Result<ReportDto>.Success(report);
    }
}
