using AuthService.Application.Common;
using MediatR;

namespace AuthService.Application.Features.Auth.ChangePassword;

public sealed record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
) : IRequest<ApiResponse<ChangePasswordResponse>>;
