using LMS.Application.Common;
using LMS.Application.DTOs.Borrows;
using LMS.Application.Interfaces;
using LMS.Application.Strategies;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;

namespace LMS.Application.Services;

/// <summary>
/// Handles UC-12 Issue Book, UC-13 Receive Book, UC-14 Fine (triggered), UC-15 View Records, UC-18 Member History.
/// Uses IFineCalculationStrategy (Strategy Pattern) for overdue fine calculation.
/// </summary>
public class BorrowService : IBorrowService
{
    private readonly IUnitOfWork _uow;
    private readonly IFineCalculationStrategy _fineStrategy;

    public BorrowService(IUnitOfWork uow, IFineCalculationStrategy fineStrategy)
    {
        _uow = uow;
        _fineStrategy = fineStrategy;
    }

    /// <summary>UC-12: Librarian issues books to a member.</summary>
    public async Task<Result<BorrowRecordDto>> IssueBookAsync(int librarianId, IssueBorrowDto dto)
    {
        // Validate member exists and is active
        var member = await _uow.Users.GetByIdAsync(dto.MemberId);
        if (member == null) return Result<BorrowRecordDto>.NotFound($"Member {dto.MemberId} not found.");
        if (!member.IsActive()) return Result<BorrowRecordDto>.BadRequest("Member account is not active.");

        // Validate all books are available
        var borrowDetails = new List<BorrowDetail>();
        foreach (var item in dto.Books)
        {
            var book = await _uow.Books.GetByIdAsync(item.BookId);
            if (book == null) return Result<BorrowRecordDto>.NotFound($"Book {item.BookId} not found.");
            if (!book.IsAvailable()) return Result<BorrowRecordDto>.BadRequest($"Book '{book.Title}' is not available.");
            if (book.Quantity < item.Quantity)
                return Result<BorrowRecordDto>.BadRequest($"Not enough copies of '{book.Title}'. Available: {book.Quantity}.");

            // Reserve: reduce book quantity
            book.Quantity -= item.Quantity;
            if (book.Quantity == 0) book.Status = "Borrowed";
            _uow.Books.Update(book);

            borrowDetails.Add(new BorrowDetail
            {
                BookId = item.BookId,
                Quantity = item.Quantity,
                Status = "Borrowed"
            });
        }

        // Create borrow record (Factory Pattern via object initializer)
        var record = new BorrowRecord
        {
            MemberId = dto.MemberId,
            LibrarianId = librarianId,
            BorrowDate = DateOnly.FromDateTime(DateTime.UtcNow),
            DueDate = dto.DueDate,
            Status = "Borrowing",
            BorrowDetails = borrowDetails
        };

        await _uow.Borrows.AddAsync(record);
        await _uow.SaveChangesAsync();

        var created = await _uow.Borrows.GetWithDetailsAsync(record.BorrowId);
        return Result<BorrowRecordDto>.Created(MapToDto(created!));
    }

    /// <summary>UC-13: Librarian receives returned book. Triggers UC-14 if overdue.</summary>
    public async Task<Result<BorrowRecordDto>> ReturnBookAsync(int borrowId, int librarianId)
    {
        var record = await _uow.Borrows.GetWithDetailsAsync(borrowId);
        if (record == null) return Result<BorrowRecordDto>.NotFound($"Borrow record {borrowId} not found.");
        if (record.Status == "Returned") return Result<BorrowRecordDto>.BadRequest("This record has already been returned.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        decimal totalFine = 0;

        foreach (var detail in record.BorrowDetails.Where(d => d.Status == "Borrowed"))
        {
            detail.Status = "Returned";
            detail.ReturnDate = today;

            // UC-14: Calculate fine if overdue
            int daysOverdue = today.DayNumber - record.DueDate.DayNumber;
            if (daysOverdue > 0)
            {
                var fineAmount = _fineStrategy.Calculate(daysOverdue);
                var fine = new Fine
                {
                    BorrowDetailId = detail.BorrowDetailId,
                    DaysOverdue = daysOverdue,
                    FineAmount = fineAmount,
                    PaidStatus = false
                };
                await _uow.Fines.AddAsync(fine);
                totalFine += fineAmount;
            }

            // Restore book availability
            if (detail.Book != null)
            {
                detail.Book.Quantity += detail.Quantity;
                detail.Book.Status = "Available";
                _uow.Books.Update(detail.Book);
            }
        }

        record.Status = "Returned";
        record.ReturnDate = today;
        record.TotalFine = totalFine;
        _uow.Borrows.Update(record);
        await _uow.SaveChangesAsync();

        return Result<BorrowRecordDto>.Success(MapToDto(record));
    }

    public async Task<Result<IEnumerable<BorrowRecordDto>>> GetAllRecordsAsync(int? memberId, DateOnly? from, DateOnly? to)
    {
        IEnumerable<BorrowRecord> records;
        if (memberId.HasValue)
            records = await _uow.Borrows.GetByMemberAsync(memberId.Value);
        else if (from.HasValue && to.HasValue)
            records = await _uow.Borrows.GetByDateRangeAsync(from.Value, to.Value);
        else
            records = await _uow.Borrows.GetActiveRecordsAsync();

        return Result<IEnumerable<BorrowRecordDto>>.Success(records.Select(MapToDto));
    }

    public async Task<Result<BorrowRecordDto>> GetRecordByIdAsync(int borrowId)
    {
        var record = await _uow.Borrows.GetWithDetailsAsync(borrowId);
        if (record == null) return Result<BorrowRecordDto>.NotFound($"Borrow record {borrowId} not found.");
        return Result<BorrowRecordDto>.Success(MapToDto(record));
    }

    public async Task<Result<IEnumerable<BorrowRecordDto>>> GetMemberHistoryAsync(int memberId)
    {
        var records = await _uow.Borrows.GetByMemberAsync(memberId);
        return Result<IEnumerable<BorrowRecordDto>>.Success(records.Select(MapToDto));
    }

    private static BorrowRecordDto MapToDto(BorrowRecord r) => new()
    {
        BorrowId = r.BorrowId,
        MemberId = r.MemberId,
        MemberName = r.Member?.FullName ?? r.Member?.Username ?? string.Empty,
        LibrarianId = r.LibrarianId,
        LibrarianName = r.Librarian?.FullName ?? r.Librarian?.Username,
        BorrowDate = r.BorrowDate,
        DueDate = r.DueDate,
        ReturnDate = r.ReturnDate,
        Status = r.Status,
        TotalFine = r.TotalFine,
        Details = r.BorrowDetails.Select(d => new BorrowDetailItemDto
        {
            BorrowDetailId = d.BorrowDetailId,
            BookId = d.BookId ?? 0,
            BookTitle = d.Book?.Title ?? string.Empty,
            Quantity = d.Quantity,
            ReturnDate = d.ReturnDate,
            Status = d.Status
        }).ToList()
    };
}
