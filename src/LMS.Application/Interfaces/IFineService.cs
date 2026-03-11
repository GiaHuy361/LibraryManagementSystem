using LMS.Application.Common;
using LMS.Application.DTOs.Fines;

namespace LMS.Application.Interfaces;

/// <summary>UC-14 Calculate Fine</summary>
public interface IFineService
{
    Task<Result<FineDto>> GetFineByBorrowDetailAsync(int borrowDetailId);
    Task<Result<FineDto>> PayFineAsync(int fineId);
    Task<Result<IEnumerable<FineDto>>> GetUnpaidFinesAsync();
}
