using AuthService.Application.Common;
using AuthService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.User.GetProfile;

public sealed class GetProfileHandler : IRequestHandler<GetProfileQuery, ApiResponse<ProfileDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<GetProfileHandler> _logger;

    public GetProfileHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<GetProfileHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<ApiResponse<ProfileDto>> Handle(
        GetProfileQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                return ApiResponse<ProfileDto>.FailResponse(
                    "User not found",
                    new List<string> { "Invalid user ID" });
            }

            var roles = await _userManager.GetRolesAsync(user);

            var profile = new ProfileDto(
                user.Id,
                user.Email!,
                user.FirstName,
                user.LastName,
                user.PhoneNumber,
                user.DateOfBirth,
                user.ProfilePictureUrl,
                user.EmailConfirmed,
                user.PhoneNumberConfirmed,
                user.IsTwoFactorEnabled,
                roles);

            return ApiResponse<ProfileDto>.SuccessResponse(profile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profile for user {UserId}", request.UserId);
            return ApiResponse<ProfileDto>.FailResponse(
                "An error occurred while retrieving profile",
                new List<string> { ex.Message });
        }
    }
}
