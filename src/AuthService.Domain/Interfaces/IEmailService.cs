namespace AuthService.Domain.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    Task SendConfirmationEmailAsync(string email, string confirmationLink, CancellationToken cancellationToken = default);
    Task SendPasswordResetEmailAsync(string email, string resetLink, CancellationToken cancellationToken = default);
    Task SendTwoFactorCodeEmailAsync(string email, string code, CancellationToken cancellationToken = default);
}
