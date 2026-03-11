namespace LMS.Domain.Entities;

/// <summary>
/// Represents a system user (Admin, Librarian, or Member).
/// </summary>
public class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int? RoleId { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Role? Role { get; set; }
    public ICollection<BorrowRecord> BorrowsAsMember { get; set; } = new List<BorrowRecord>();
    public ICollection<BorrowRecord> BorrowsAsLibrarian { get; set; } = new List<BorrowRecord>();

    /// <summary>
    /// Checks if this user account is active.
    /// </summary>
    public bool IsActive() => Status.Equals("Active", StringComparison.OrdinalIgnoreCase);
}
