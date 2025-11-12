using AuthService.Domain.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace AuthService.Infrastructure.Services;

public sealed class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _configuration["EmailSettings:SenderName"] ?? "AuthService",
                _configuration["EmailSettings:SenderEmail"] ?? "noreply@authservice.com"
            ));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            
            var host = _configuration["EmailSettings:SmtpHost"] ?? "localhost";
            var port = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls, cancellationToken);
            
            var username = _configuration["EmailSettings:SmtpUsername"];
            var password = _configuration["EmailSettings:SmtpPassword"];
            
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                await client.AuthenticateAsync(username, password, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {To}", to);
            throw;
        }
    }

    public async Task SendConfirmationEmailAsync(string email, string confirmationLink, CancellationToken cancellationToken = default)
    {
        var subject = "Confirm your email";
        var body = $@"
            <html>
            <body>
                <h2>Email Confirmation</h2>
                <p>Thank you for registering with AuthService.</p>
                <p>Please confirm your email address by clicking the link below:</p>
                <p><a href='{confirmationLink}'>Confirm Email</a></p>
                <p>If you didn't create an account, please ignore this email.</p>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body, cancellationToken);
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetLink, CancellationToken cancellationToken = default)
    {
        var subject = "Reset your password";
        var body = $@"
            <html>
            <body>
                <h2>Password Reset</h2>
                <p>You have requested to reset your password.</p>
                <p>Please click the link below to reset your password:</p>
                <p><a href='{resetLink}'>Reset Password</a></p>
                <p>If you didn't request this, please ignore this email.</p>
                <p>This link will expire in 24 hours.</p>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body, cancellationToken);
    }

    public async Task SendTwoFactorCodeEmailAsync(string email, string code, CancellationToken cancellationToken = default)
    {
        var subject = "Your two-factor authentication code";
        var body = $@"
            <html>
            <body>
                <h2>Two-Factor Authentication</h2>
                <p>Your verification code is:</p>
                <h1>{code}</h1>
                <p>This code will expire in 5 minutes.</p>
                <p>If you didn't request this code, please contact support immediately.</p>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body, cancellationToken);
    }
}
