using AuthService.Application.Common;
using AuthService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Auth.VerifyEmail;

public sealed class VerifyEmailHandler : IRequestHandler<VerifyEmailCommand, ApiResponse<VerifyEmailResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<VerifyEmailHandler> _logger;

    public VerifyEmailHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<VerifyEmailHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<ApiResponse<VerifyEmailResponse>> Handle(
        VerifyEmailCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                return ApiResponse<VerifyEmailResponse>.FailResponse(
                    "User not found",
                    new List<string> { "Invalid user ID" });
            }

            if (user.EmailConfirmed)
            {
                return ApiResponse<VerifyEmailResponse>.FailResponse(
                    "Email already confirmed",
                    new List<string> { "Email has already been verified" });
            }

            var result = await _userManager.ConfirmEmailAsync(user, request.Token);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponse<VerifyEmailResponse>.FailResponse(
                    "Email verification failed",
                    errors);
            }

            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("Email verified for user {UserId}", user.Id);

            var response = new VerifyEmailResponse(true, "Email verified successfully");
            return ApiResponse<VerifyEmailResponse>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during email verification for user {UserId}", request.UserId);
            return ApiResponse<VerifyEmailResponse>.FailResponse(
                "An error occurred during email verification",
                new List<string> { ex.Message });
        }
    }
}
