using AuthService.Application.Common;
using MediatR;

namespace AuthService.Application.Features.Auth.ResetPassword;

public sealed record ResetPasswordCommand(
    Guid UserId,
    string Token,
    string NewPassword,
    string ConfirmPassword
) : IRequest<ApiResponse<ResetPasswordResponse>>;
