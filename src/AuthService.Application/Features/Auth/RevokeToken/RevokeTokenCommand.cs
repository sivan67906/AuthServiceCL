using AuthService.Application.Common;
using MediatR;

namespace AuthService.Application.Features.Auth.RevokeToken;

public sealed record RevokeTokenCommand(
    Guid UserId
) : IRequest<ApiResponse<RevokeTokenResponse>>;
