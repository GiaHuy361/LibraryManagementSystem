namespace LMS.Domain.Entities;

/// <summary>
/// Represents a book category/genre.
/// </summary>
public class Category
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
