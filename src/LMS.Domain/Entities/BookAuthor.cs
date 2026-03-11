namespace LMS.Domain.Entities;

/// <summary>
/// Join table for the many-to-many relationship between Book and Author.
/// </summary>
public class BookAuthor
{
    public int BookId { get; set; }
    public int AuthorId { get; set; }

    // Navigation
    public Book Book { get; set; } = null!;
    public Author Author { get; set; } = null!;
}
