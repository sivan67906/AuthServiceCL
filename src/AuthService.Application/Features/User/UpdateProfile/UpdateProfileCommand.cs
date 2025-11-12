using AuthService.Application.Common;
using MediatR;

namespace AuthService.Application.Features.User.UpdateProfile;

public sealed record UpdateProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    DateTime? DateOfBirth,
    string? ProfilePictureUrl
) : IRequest<ApiResponse<UpdateProfileResponse>>;
