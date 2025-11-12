namespace AuthService.Application.Features.Auth.VerifyEmail;

public sealed record VerifyEmailResponse(
    bool Success,
    string Message
);
