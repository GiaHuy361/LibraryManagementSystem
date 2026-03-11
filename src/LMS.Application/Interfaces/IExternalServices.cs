namespace LMS.Application.Interfaces;

/// <summary>
/// External service interface for JWT token generation.
/// Implemented in Infrastructure, consumed in Application.
/// </summary>
public interface IJwtTokenService
{
    string GenerateToken(int userId, string username, string role);
}

/// <summary>
/// External service interface for sending email notifications.
/// Stub implementation in Infrastructure (ready for SMTP integration).
/// </summary>
public interface IEmailService
{
    Task SendOverdueReminderAsync(string toEmail, string memberName, string bookTitle, DateOnly dueDate);
}
