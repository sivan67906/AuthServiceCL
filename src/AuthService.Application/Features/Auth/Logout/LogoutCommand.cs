using AuthService.Application.Common;
using MediatR;

namespace AuthService.Application.Features.Auth.Logout;

public sealed record LogoutCommand(
    Guid UserId
) : IRequest<ApiResponse<LogoutResponse>>;
