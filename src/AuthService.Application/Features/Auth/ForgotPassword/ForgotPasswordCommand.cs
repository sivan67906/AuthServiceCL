using AuthService.Application.Common;
using MediatR;

namespace AuthService.Application.Features.Auth.ForgotPassword;

public sealed record ForgotPasswordCommand(
    string Email
) : IRequest<ApiResponse<ForgotPasswordResponse>>;
