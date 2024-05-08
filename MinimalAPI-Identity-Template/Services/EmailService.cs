using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using MinimalApiIdentityTemplate.Models;

namespace MinimalApiIdentityTemplate.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(EmailDto request)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(request.From));
        email.To.Add(MailboxAddress.Parse(request.To));
        email.Subject = request.Subject;
        email.Body = new TextPart(TextFormat.Html) { Text = request.Body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_config["EmailHost"], 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_config["EmailUsername"], _config["EmailPassword"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}