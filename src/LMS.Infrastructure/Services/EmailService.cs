using LMS.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace LMS.Infrastructure.Services;

/// <summary>
/// External service stub: Email notifications.
/// Currently logs to console. Replace SmtpClient logic when SMTP is available.
/// </summary>
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _config;

    public EmailService(ILogger<EmailService> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    public async Task SendOverdueReminderAsync(string toEmail, string memberName, string bookTitle, DateOnly dueDate)
    {
        var emailSettings = _config.GetSection("EmailSettings");
        var host = emailSettings["Host"];
        var port = int.Parse(emailSettings["Port"] ?? "587");
        var username = emailSettings["Username"];
        var password = emailSettings["Password"];
        var fromEmail = emailSettings["FromEmail"];
        
        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username))
        {
            _logger.LogWarning("[EMAIL STUB] SMTP not fully configured. Simulating email to {Email}", toEmail);
            return;
        }

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Library Management System", fromEmail));
            message.To.Add(new MailboxAddress(memberName, toEmail));
            message.Subject = $"Overdue Book Reminder: {bookTitle}";

            message.Body = new TextPart("plain")
            {
                Text = $@"Hello {memberName},

This is a reminder that the book '{bookTitle}' was due on {dueDate}.
Please return it as soon as possible to avoid accumulating further fines.

Thank you,
Library Management System"
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(username, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            
            _logger.LogInformation("Overdue reminder successfully sent to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send overdue email to {Email}", toEmail);
        }
    }
}
