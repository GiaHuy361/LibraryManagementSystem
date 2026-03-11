namespace LMS.Domain.Entities;

/// <summary>
/// Represents a user role in the system (Admin, Librarian, Member).
/// </summary>
public class Role
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation
    public ICollection<User> Users { get; set; } = new List<User>();
}
