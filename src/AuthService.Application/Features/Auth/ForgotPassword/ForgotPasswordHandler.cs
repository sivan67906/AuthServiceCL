using AuthService.Application.Common;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Auth.ForgotPassword;

public sealed class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, ApiResponse<ForgotPasswordResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<ForgotPasswordHandler> _logger;

    public ForgotPasswordHandler(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        ILogger<ForgotPasswordHandler> logger)
    {
        _userManager = userManager;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<ApiResponse<ForgotPasswordResponse>> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !user.EmailConfirmed)
            {
                return ApiResponse<ForgotPasswordResponse>.SuccessResponse(
                    new ForgotPasswordResponse("If the email exists, a password reset link has been sent"),
                    "Password reset email sent");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"https://localhost:5001/api/auth/reset-password?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            
            await _emailService.SendPasswordResetEmailAsync(user.Email!, resetLink, cancellationToken);

            _logger.LogInformation("Password reset email sent to {Email}", user.Email);

            var response = new ForgotPasswordResponse("If the email exists, a password reset link has been sent");
            return ApiResponse<ForgotPasswordResponse>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forgot password for {Email}", request.Email);
            return ApiResponse<ForgotPasswordResponse>.FailResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message });
        }
    }
}
