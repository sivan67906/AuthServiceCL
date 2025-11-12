using AuthService.Application.Common;
using AuthService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.TwoFactor.Verify;

public sealed class Verify2FAHandler : IRequestHandler<Verify2FACommand, ApiResponse<Verify2FAResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<Verify2FAHandler> _logger;

    public Verify2FAHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<Verify2FAHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<ApiResponse<Verify2FAResponse>> Handle(
        Verify2FACommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                return ApiResponse<Verify2FAResponse>.FailResponse(
                    "User not found",
                    new List<string> { "Invalid user ID" });
            }

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                request.Code);

            if (!isValid)
            {
                return ApiResponse<Verify2FAResponse>.FailResponse(
                    "Invalid verification code",
                    new List<string> { "The code you entered is incorrect" });
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);

            user.IsTwoFactorEnabled = true;
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

            _logger.LogInformation("2FA verified and enabled for user {UserId}", user.Id);

            var response = new Verify2FAResponse(
                true,
                "Two-factor authentication enabled successfully",
                recoveryCodes);

            return ApiResponse<Verify2FAResponse>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying 2FA for user {UserId}", request.UserId);
            return ApiResponse<Verify2FAResponse>.FailResponse(
                "An error occurred while verifying 2FA",
                new List<string> { ex.Message });
        }
    }
}
