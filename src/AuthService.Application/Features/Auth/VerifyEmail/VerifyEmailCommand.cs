using AuthService.Application.Common;
using MediatR;

namespace AuthService.Application.Features.Auth.VerifyEmail;

public sealed record VerifyEmailCommand(
    Guid UserId,
    string Token
) : IRequest<ApiResponse<VerifyEmailResponse>>;
