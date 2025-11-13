namespace AuthService.Application.Features.UserAddress.AddAddress;

public sealed record AddUserAddressResponse(
    Guid AddressId,
    string Message
);
