using AuthService.Application.Common;
using AuthService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Auth.Logout;

public sealed class LogoutHandler : IRequestHandler<LogoutCommand, ApiResponse<LogoutResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<LogoutHandler> _logger;

    public LogoutHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<LogoutHandler> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    public async Task<ApiResponse<LogoutResponse>> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                return ApiResponse<LogoutResponse>.FailResponse(
                    "User not found",
                    new List<string> { "Invalid user ID" });
            }

            // Revoke refresh token
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            // Sign out
            await _signInManager.SignOutAsync();

            _logger.LogInformation("User {UserId} logged out successfully", user.Id);

            var response = new LogoutResponse("Logged out successfully");
            return ApiResponse<LogoutResponse>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout for user {UserId}", request.UserId);
            return ApiResponse<LogoutResponse>.FailResponse(
                "An error occurred during logout",
                new List<string> { ex.Message });
        }
    }
}
