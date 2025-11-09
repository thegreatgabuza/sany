using Microsoft.AspNetCore.Identity.UI.Services;

namespace Cascade.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // For development purposes, we'll just log the email content
            // In production, you would integrate with an actual email service like SendGrid, AWS SES, etc.
            Console.WriteLine($"Email to: {email}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {htmlMessage}");
            Console.WriteLine("--- End of Email ---");
            
            // Return a completed task since we're not actually sending emails in development
            return Task.CompletedTask;
        }
    }
}
