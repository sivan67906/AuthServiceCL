namespace AuthService.Application.Features.UserAddress.GetAddresses;

public sealed record GetUserAddressesResponse(
    List<UserAddressDto> Addresses
);

public sealed record UserAddressDto(
    Guid Id,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string State,
    string PostalCode,
    string Country,
    bool IsPrimary,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
