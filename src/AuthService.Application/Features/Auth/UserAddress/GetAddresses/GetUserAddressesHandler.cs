using AuthService.Application.Common;
using AuthService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.UserAddress.GetAddresses;

public sealed class GetUserAddressesHandler : IRequestHandler<GetUserAddressesQuery, ApiResponse<GetUserAddressesResponse>>
{
    private readonly IUserAddressRepository _addressRepository;
    private readonly ILogger<GetUserAddressesHandler> _logger;

    public GetUserAddressesHandler(
        IUserAddressRepository addressRepository,
        ILogger<GetUserAddressesHandler> logger)
    {
        _addressRepository = addressRepository;
        _logger = logger;
    }

    public async Task<ApiResponse<GetUserAddressesResponse>> Handle(
        GetUserAddressesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var addresses = await _addressRepository.GetByUserIdAsync(request.UserId, cancellationToken);

            var addressDtos = addresses.Select(a => new UserAddressDto(
                a.Id,
                a.AddressLine1,
                a.AddressLine2,
                a.City,
                a.State,
                a.PostalCode,
                a.Country,
                a.IsPrimary,
                a.CreatedAt,
                a.UpdatedAt
            )).ToList();

            var response = new GetUserAddressesResponse(addressDtos);
            return ApiResponse<GetUserAddressesResponse>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving addresses for user {UserId}", request.UserId);
            return ApiResponse<GetUserAddressesResponse>.FailResponse(
                "An error occurred while retrieving addresses",
                new List<string> { ex.Message });
        }
    }
}
