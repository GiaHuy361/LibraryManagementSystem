namespace LMS.Domain.Entities;

/// <summary>
/// Represents a borrow transaction (header) for a member.
/// </summary>
public class BorrowRecord
{
    public int BorrowId { get; set; }
    public int MemberId { get; set; }
    public int? LibrarianId { get; set; }
    public DateOnly BorrowDate { get; set; }
    public DateOnly DueDate { get; set; }
    public DateOnly? ReturnDate { get; set; }
    public string Status { get; set; } = "Borrowing"; // Borrowing | Returned | Overdue
    public decimal TotalFine { get; set; } = 0;

    // Navigation
    public User Member { get; set; } = null!;
    public User? Librarian { get; set; }
    public ICollection<BorrowDetail> BorrowDetails { get; set; } = new List<BorrowDetail>();

    /// <summary>
    /// Determines if the borrow record is overdue relative to today.
    /// </summary>
    public bool IsOverdue() => ReturnDate == null && DateOnly.FromDateTime(DateTime.UtcNow) > DueDate;
}
