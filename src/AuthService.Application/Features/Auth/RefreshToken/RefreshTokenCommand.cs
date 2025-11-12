using AuthService.Application.Common;
using MediatR;

namespace AuthService.Application.Features.Auth.RefreshToken;

public sealed record RefreshTokenCommand(
    string AccessToken,
    string RefreshToken
) : IRequest<ApiResponse<RefreshTokenResponse>>;
