using AuthService.Application.Common;
using MediatR;

namespace AuthService.Application.Features.UserAddress.DeleteAddress;

public sealed record DeleteUserAddressCommand(
    Guid AddressId,
    Guid UserId
) : IRequest<ApiResponse<DeleteUserAddressResponse>>;
