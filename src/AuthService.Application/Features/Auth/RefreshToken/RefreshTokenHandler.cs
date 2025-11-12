using AuthService.Application.Common;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace AuthService.Application.Features.Auth.RefreshToken;

public sealed class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<RefreshTokenResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly ILogger<RefreshTokenHandler> _logger;

    public RefreshTokenHandler(
        UserManager<ApplicationUser> userManager,
        IJwtService jwtService,
        ILogger<RefreshTokenHandler> logger)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<ApiResponse<RefreshTokenResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null)
            {
                return ApiResponse<RefreshTokenResponse>.FailResponse(
                    "Invalid access token",
                    new List<string> { "Token validation failed" });
            }

            var userIdClaim = principal.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return ApiResponse<RefreshTokenResponse>.FailResponse(
                    "Invalid token claims",
                    new List<string> { "User ID not found in token" });
            }

            var user = await _userManager.FindByIdAsync(userIdClaim);
            if (user == null)
            {
                return ApiResponse<RefreshTokenResponse>.FailResponse(
                    "User not found",
                    new List<string> { "Invalid user" });
            }

            if (user.RefreshToken != request.RefreshToken)
            {
                return ApiResponse<RefreshTokenResponse>.FailResponse(
                    "Invalid refresh token",
                    new List<string> { "Token mismatch" });
            }

            if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return ApiResponse<RefreshTokenResponse>.FailResponse(
                    "Refresh token expired",
                    new List<string> { "Token has expired" });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email!),
                new(ClaimTypes.Name, user.FullName),
                new("UserId", user.Id.ToString())
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var newAccessToken = _jwtService.GenerateAccessToken(claims);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("Token refreshed for user {UserId}", user.Id);

            var response = new RefreshTokenResponse(
                newAccessToken,
                newRefreshToken,
                _jwtService.GetTokenExpiryTime());

            return ApiResponse<RefreshTokenResponse>.SuccessResponse(response, "Token refreshed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return ApiResponse<RefreshTokenResponse>.FailResponse(
                "An error occurred during token refresh",
                new List<string> { ex.Message });
        }
    }
}
