using Microsoft.AspNetCore.Identity.UI.Services;

namespace PollBuilder.Infrastructure.Services;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Không gửi email
        return Task.CompletedTask;
    }
}