using AuthService.Application.Common;
using AuthService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Auth.ChangePassword;

public sealed class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, ApiResponse<ChangePasswordResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ChangePasswordHandler> _logger;

    public ChangePasswordHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<ChangePasswordHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<ApiResponse<ChangePasswordResponse>> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                return ApiResponse<ChangePasswordResponse>.FailResponse(
                    "User not found",
                    new List<string> { "Invalid user ID" });
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                request.CurrentPassword,
                request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponse<ChangePasswordResponse>.FailResponse(
                    "Password change failed",
                    errors);
            }

            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("Password changed successfully for user {UserId}", user.Id);

            var response = new ChangePasswordResponse("Password changed successfully");
            return ApiResponse<ChangePasswordResponse>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password change for user {UserId}", request.UserId);
            return ApiResponse<ChangePasswordResponse>.FailResponse(
                "An error occurred during password change",
                new List<string> { ex.Message });
        }
    }
}
