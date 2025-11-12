namespace AuthService.Application.Features.Auth.Register;

public sealed record RegisterResponse(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string Message
);
