using AuthService.Application.Common;
using AuthService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Text;

namespace AuthService.Application.Features.TwoFactor.Enable;

public sealed class Enable2FAHandler : IRequestHandler<Enable2FACommand, ApiResponse<Enable2FAResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<Enable2FAHandler> _logger;

    public Enable2FAHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<Enable2FAHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<ApiResponse<Enable2FAResponse>> Handle(
        Enable2FACommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                return ApiResponse<Enable2FAResponse>.FailResponse(
                    "User not found",
                    new List<string> { "Invalid user ID" });
            }

            if (user.IsTwoFactorEnabled)
            {
                return ApiResponse<Enable2FAResponse>.FailResponse(
                    "Two-factor authentication is already enabled",
                    new List<string> { "2FA already active" });
            }

            await _userManager.ResetAuthenticatorKeyAsync(user);
            var sharedKey = await _userManager.GetAuthenticatorKeyAsync(user);

            if (string.IsNullOrEmpty(sharedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                sharedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var qrCodeUri = GenerateQrCodeUri(user.Email!, sharedKey!);

            user.TwoFactorSecretKey = sharedKey;
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("2FA setup initiated for user {UserId}", user.Id);

            var response = new Enable2FAResponse(
                FormatKey(sharedKey!),
                qrCodeUri,
                "Scan the QR code with your authenticator app");

            return ApiResponse<Enable2FAResponse>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enabling 2FA for user {UserId}", request.UserId);
            return ApiResponse<Enable2FAResponse>.FailResponse(
                "An error occurred while enabling 2FA",
                new List<string> { ex.Message });
        }
    }

    private static string FormatKey(string unformattedKey)
    {
        var result = new StringBuilder();
        int currentPosition = 0;
        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
            currentPosition += 4;
        }
        if (currentPosition < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition));
        }

        return result.ToString().ToLowerInvariant();
    }

    private static string GenerateQrCodeUri(string email, string sharedKey)
    {
        const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        return string.Format(
            AuthenticatorUriFormat,
            Uri.EscapeDataString("AuthService"),
            Uri.EscapeDataString(email),
            sharedKey);
    }
}
