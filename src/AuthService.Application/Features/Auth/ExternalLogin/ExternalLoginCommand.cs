using AuthService.Application.Common;
using AuthService.Domain.Enums;
using MediatR;

namespace AuthService.Application.Features.Auth.ExternalLogin;

public sealed record ExternalLoginCommand(
    ExternalProvider Provider,
    string ProviderKey,
    string Email,
    string? FirstName,
    string? LastName
) : IRequest<ApiResponse<ExternalLoginResponse>>;
