namespace LMS.Application.DTOs.Borrows;

public class BorrowDetailItemDto
{
    public int BorrowDetailId { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateOnly? ReturnDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class BorrowRecordDto
{
    public int BorrowId { get; set; }
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public int? LibrarianId { get; set; }
    public string? LibrarianName { get; set; }
    public DateOnly BorrowDate { get; set; }
    public DateOnly DueDate { get; set; }
    public DateOnly? ReturnDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalFine { get; set; }
    public List<BorrowDetailItemDto> Details { get; set; } = new();
}

public class IssueBorrowDto
{
    public int MemberId { get; set; }
    public DateOnly DueDate { get; set; }
    public List<BorrowBookItemDto> Books { get; set; } = new();
}

public class BorrowBookItemDto
{
    public int BookId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class ReturnBorrowDto
{
    public int BorrowDetailId { get; set; }
}
