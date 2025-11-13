using AuthService.Application.Common;
using MediatR;

namespace AuthService.Application.Features.UserAddress.AddAddress;

public sealed record AddUserAddressCommand(
    Guid UserId,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string State,
    string PostalCode,
    string Country,
    bool IsPrimary
) : IRequest<ApiResponse<AddUserAddressResponse>>;
