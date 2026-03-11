using LMS.Application.Common;
using LMS.Application.DTOs.Fines;

namespace LMS.Application.Interfaces;

/// <summary>UC-11 View Reports</summary>
public interface IReportService
{
    Task<Result<ReportDto>> GetSummaryReportAsync();
}
