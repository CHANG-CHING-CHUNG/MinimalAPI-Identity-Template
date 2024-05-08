using MinimalApiIdentityTemplate.Models;

namespace MinimalApiIdentityTemplate.Services;

public interface IEmailService
{
    Task SendEmailAsync(EmailDto request);
}