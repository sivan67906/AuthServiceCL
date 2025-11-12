using AuthService.Application.Common;

namespace AuthService.Application.Features.User.GetProfile;

public sealed record ProfileDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    DateTime? DateOfBirth,
    string? ProfilePictureUrl,
    bool EmailConfirmed,
    bool PhoneNumberConfirmed,
    bool TwoFactorEnabled,
    IEnumerable<string> Roles
);
