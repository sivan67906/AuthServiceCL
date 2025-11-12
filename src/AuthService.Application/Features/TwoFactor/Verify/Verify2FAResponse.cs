namespace AuthService.Application.Features.TwoFactor.Verify;

public sealed record Verify2FAResponse(
    bool Success,
    string Message,
    IEnumerable<string>? RecoveryCodes
);
