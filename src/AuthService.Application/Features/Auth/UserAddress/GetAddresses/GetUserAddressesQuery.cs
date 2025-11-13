using AuthService.Application.Common;
using MediatR;

namespace AuthService.Application.Features.UserAddress.GetAddresses;

public sealed record GetUserAddressesQuery(
    Guid UserId
) : IRequest<ApiResponse<GetUserAddressesResponse>>;
