using AuthService.Application.Common;
using MediatR;

namespace AuthService.Application.Features.TwoFactor.Enable;

public sealed record Enable2FACommand(
    Guid UserId
) : IRequest<ApiResponse<Enable2FAResponse>>;
