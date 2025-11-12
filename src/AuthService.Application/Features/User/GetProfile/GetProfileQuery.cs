using AuthService.Application.Common;
using MediatR;

namespace AuthService.Application.Features.User.GetProfile;

public sealed record GetProfileQuery(
    Guid UserId
) : IRequest<ApiResponse<ProfileDto>>;
