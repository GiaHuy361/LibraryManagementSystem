namespace LMS.Application.DTOs.Authors;

public class AuthorDto
{
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? Biography { get; set; }
}

public class CreateAuthorDto
{
    public string AuthorName { get; set; } = string.Empty;
    public string? Biography { get; set; }
}

public class UpdateAuthorDto
{
    public string? AuthorName { get; set; }
    public string? Biography { get; set; }
}
