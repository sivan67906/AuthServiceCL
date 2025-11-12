using AuthService.Application.Common;
using AuthService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.User.UpdateProfile;

public sealed class UpdateProfileHandler : IRequestHandler<UpdateProfileCommand, ApiResponse<UpdateProfileResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UpdateProfileHandler> _logger;

    public UpdateProfileHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<UpdateProfileHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<ApiResponse<UpdateProfileResponse>> Handle(
        UpdateProfileCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                return ApiResponse<UpdateProfileResponse>.FailResponse(
                    "User not found",
                    new List<string> { "Invalid user ID" });
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.DateOfBirth = request.DateOfBirth;
            user.ProfilePictureUrl = request.ProfilePictureUrl;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponse<UpdateProfileResponse>.FailResponse(
                    "Profile update failed",
                    errors);
            }

            _logger.LogInformation("Profile updated for user {UserId}", user.Id);

            var response = new UpdateProfileResponse("Profile updated successfully");
            return ApiResponse<UpdateProfileResponse>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for user {UserId}", request.UserId);
            return ApiResponse<UpdateProfileResponse>.FailResponse(
                "An error occurred while updating profile",
                new List<string> { ex.Message });
        }
    }
}
