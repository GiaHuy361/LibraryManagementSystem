using LMS.Application.Common;
using LMS.Application.DTOs.Borrows;

namespace LMS.Application.Interfaces;

/// <summary>UC-12 Issue Book / UC-13 Receive Book / UC-15 View Records / UC-18 Member History</summary>
public interface IBorrowService
{
    Task<Result<BorrowRecordDto>> IssueBookAsync(int librarianId, IssueBorrowDto dto);
    Task<Result<BorrowRecordDto>> ReturnBookAsync(int borrowId, int librarianId);
    Task<Result<IEnumerable<BorrowRecordDto>>> GetAllRecordsAsync(int? memberId, DateOnly? from, DateOnly? to);
    Task<Result<BorrowRecordDto>> GetRecordByIdAsync(int borrowId);
    Task<Result<IEnumerable<BorrowRecordDto>>> GetMemberHistoryAsync(int memberId);
}
