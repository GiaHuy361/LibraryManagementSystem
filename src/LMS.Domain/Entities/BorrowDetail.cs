namespace LMS.Domain.Entities;

/// <summary>
/// Represents a single book line item within a BorrowRecord.
/// </summary>
public class BorrowDetail
{
    public int BorrowDetailId { get; set; }
    public int? BorrowId { get; set; }
    public int? BookId { get; set; }
    public int Quantity { get; set; } = 1;
    public DateOnly? ReturnDate { get; set; }
    public string Status { get; set; } = "Borrowed"; // Borrowed | Returned

    // Navigation
    public BorrowRecord? BorrowRecord { get; set; }
    public Book? Book { get; set; }
    public Fine? Fine { get; set; }
}
