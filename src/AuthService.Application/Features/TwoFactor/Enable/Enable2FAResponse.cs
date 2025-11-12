namespace AuthService.Application.Features.TwoFactor.Enable;

public sealed record Enable2FAResponse(
    string SharedKey,
    string QrCodeUri,
    string Message
);
