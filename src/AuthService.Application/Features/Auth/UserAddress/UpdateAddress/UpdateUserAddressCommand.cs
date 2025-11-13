using AuthService.Application.Common;
using MediatR;

namespace AuthService.Application.Features.UserAddress.UpdateAddress;

public sealed record UpdateUserAddressCommand(
    Guid AddressId,
    Guid UserId,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string State,
    string PostalCode,
    string Country,
    bool IsPrimary
) : IRequest<ApiResponse<UpdateUserAddressResponse>>;
