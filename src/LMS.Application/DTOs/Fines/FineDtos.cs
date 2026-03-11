namespace LMS.Application.DTOs.Fines;

public class FineDto
{
    public int FineId { get; set; }
    public int BorrowDetailId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public int DaysOverdue { get; set; }
    public decimal FineAmount { get; set; }
    public bool PaidStatus { get; set; }
    public DateOnly? PaidDate { get; set; }
}

public class ReportDto
{
    public int TotalBooks { get; set; }
    public int TotalBorrows { get; set; }
    public int ActiveBorrows { get; set; }
    public int OverdueRecords { get; set; }
    public decimal TotalFinesCollected { get; set; }
    public decimal TotalFinesPending { get; set; }
}
