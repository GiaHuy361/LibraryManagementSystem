namespace LMS.Domain.Entities;

/// <summary>
/// Represents an overdue fine linked to a specific BorrowDetail.
/// </summary>
public class Fine
{
    public int FineId { get; set; }
    public int? BorrowDetailId { get; set; }
    public int DaysOverdue { get; set; }
    public decimal FineAmount { get; set; }
    public bool PaidStatus { get; set; } = false;
    public DateOnly? PaidDate { get; set; }

    // Navigation
    public BorrowDetail? BorrowDetail { get; set; }

    /// <summary>
    /// Marks the fine as paid on the given date.
    /// </summary>
    public void MarkAsPaid(DateOnly paidDate)
    {
        PaidStatus = true;
        PaidDate = paidDate;
    }
}
