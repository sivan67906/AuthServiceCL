using AuthService.Application.Common;
using MediatR;

namespace AuthService.Application.Features.TwoFactor.Verify;

public sealed record Verify2FACommand(
    Guid UserId,
    string Code
) : IRequest<ApiResponse<Verify2FAResponse>>;
