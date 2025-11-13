namespace AuthService.Application.Features.Auth.ExternalLogin;

public sealed record ExternalLoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserDto User
);

public sealed record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    IEnumerable<string> Roles
);
