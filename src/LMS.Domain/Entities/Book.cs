namespace LMS.Domain.Entities;

/// <summary>
/// Represents a book in the library.
/// </summary>
public class Book
{
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Isbn { get; set; }
    public int? CategoryId { get; set; }
    public string? Publisher { get; set; }
    public int? PublishYear { get; set; }
    public string? Description { get; set; }
    public int Quantity { get; set; } = 1;
    public string Status { get; set; } = "Available";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Category? Category { get; set; }
    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    public ICollection<BorrowDetail> BorrowDetails { get; set; } = new List<BorrowDetail>();

    /// <summary>
    /// Returns true if the book has available copies for borrowing.
    /// </summary>
    public bool IsAvailable() => Quantity > 0 && Status == "Available";
}
