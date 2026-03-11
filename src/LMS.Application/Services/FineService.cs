using LMS.Application.Common;
using LMS.Application.DTOs.Fines;
using LMS.Application.Interfaces;
using LMS.Domain.Interfaces;

namespace LMS.Application.Services;

/// <summary>Handles UC-14 Fine: view, confirm payment.</summary>
public class FineService : IFineService
{
    private readonly IUnitOfWork _uow;
    public FineService(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<FineDto>> GetFineByBorrowDetailAsync(int borrowDetailId)
    {
        var fine = await _uow.Fines.GetByBorrowDetailAsync(borrowDetailId);
        if (fine == null) return Result<FineDto>.NotFound($"No fine found for borrow detail {borrowDetailId}.");
        return Result<FineDto>.Success(MapToDto(fine));
    }

    public async Task<Result<FineDto>> PayFineAsync(int fineId)
    {
        var fine = await _uow.Fines.GetByIdAsync(fineId);
        if (fine == null) return Result<FineDto>.NotFound($"Fine {fineId} not found.");
        if (fine.PaidStatus) return Result<FineDto>.BadRequest("This fine has already been paid.");

        fine.MarkAsPaid(DateOnly.FromDateTime(DateTime.UtcNow));
        _uow.Fines.Update(fine);
        await _uow.SaveChangesAsync();
        return Result<FineDto>.Success(MapToDto(fine));
    }

    public async Task<Result<IEnumerable<FineDto>>> GetUnpaidFinesAsync()
    {
        var fines = await _uow.Fines.GetUnpaidFinesAsync();
        return Result<IEnumerable<FineDto>>.Success(fines.Select(MapToDto));
    }

    private static FineDto MapToDto(LMS.Domain.Entities.Fine f) => new()
    {
        FineId = f.FineId,
        BorrowDetailId = f.BorrowDetailId ?? 0,
        BookTitle = f.BorrowDetail?.Book?.Title ?? string.Empty,
        DaysOverdue = f.DaysOverdue,
        FineAmount = f.FineAmount,
        PaidStatus = f.PaidStatus,
        PaidDate = f.PaidDate
    };
}
