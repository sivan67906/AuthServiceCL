using AuthService.Application.Common;
using MediatR;

namespace AuthService.Application.Features.Auth.Login;

public sealed record LoginCommand(
    string Email,
    string Password,
    bool RememberMe
) : IRequest<ApiResponse<LoginResponse>>;
