using LMS.Application.Common;

namespace LMS.Application.DTOs.Books;

public class BookDto
{
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Isbn { get; set; }
    public string? CategoryName { get; set; }
    public int? CategoryId { get; set; }
    public string? Publisher { get; set; }
    public int? PublishYear { get; set; }
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<string> Authors { get; set; } = new();
}

public class CreateBookDto
{
    public string Title { get; set; } = string.Empty;
    public string? Isbn { get; set; }
    public int? CategoryId { get; set; }
    public string? Publisher { get; set; }
    public int? PublishYear { get; set; }
    public string? Description { get; set; }
    public int Quantity { get; set; } = 1;
    public List<int> AuthorIds { get; set; } = new();
}

public class UpdateBookDto
{
    public string? Title { get; set; }
    public string? Isbn { get; set; }
    public int? CategoryId { get; set; }
    public string? Publisher { get; set; }
    public int? PublishYear { get; set; }
    public string? Description { get; set; }
    public int? Quantity { get; set; }
    public string? Status { get; set; }
    public List<int>? AuthorIds { get; set; }
}

public class BookSearchDto : PaginationParams
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public int? CategoryId { get; set; }
}
