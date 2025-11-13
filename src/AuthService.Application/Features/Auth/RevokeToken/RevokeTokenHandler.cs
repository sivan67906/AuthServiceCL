using AuthService.Application.Common;
using AuthService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Auth.RevokeToken;

public sealed class RevokeTokenHandler : IRequestHandler<RevokeTokenCommand, ApiResponse<RevokeTokenResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RevokeTokenHandler> _logger;

    public RevokeTokenHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<RevokeTokenHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<ApiResponse<RevokeTokenResponse>> Handle(
        RevokeTokenCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                return ApiResponse<RevokeTokenResponse>.FailResponse(
                    "User not found",
                    new List<string> { "Invalid user ID" });
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("Refresh token revoked for user {UserId}", user.Id);

            var response = new RevokeTokenResponse("Refresh token revoked successfully");
            return ApiResponse<RevokeTokenResponse>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking token for user {UserId}", request.UserId);
            return ApiResponse<RevokeTokenResponse>.FailResponse(
                "An error occurred while revoking token",
                new List<string> { ex.Message });
        }
    }
}
