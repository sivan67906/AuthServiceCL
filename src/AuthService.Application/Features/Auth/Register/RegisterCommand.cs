using AuthService.Application.Common;
using MediatR;

namespace AuthService.Application.Features.Auth.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string ConfirmPassword,
    string FirstName,
    string LastName,
    string? PhoneNumber
) : IRequest<ApiResponse<RegisterResponse>>;
