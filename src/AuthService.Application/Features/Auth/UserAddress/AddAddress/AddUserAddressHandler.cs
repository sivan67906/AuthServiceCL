using AuthService.Application.Common;
using AuthService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.UserAddress.AddAddress;

public sealed class AddUserAddressHandler : IRequestHandler<AddUserAddressCommand, ApiResponse<AddUserAddressResponse>>
{
    private readonly IUserAddressRepository _addressRepository;
    private readonly ILogger<AddUserAddressHandler> _logger;

    public AddUserAddressHandler(
        IUserAddressRepository addressRepository,
        ILogger<AddUserAddressHandler> logger)
    {
        _addressRepository = addressRepository;
        _logger = logger;
    }

    public async Task<ApiResponse<AddUserAddressResponse>> Handle(
        AddUserAddressCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // If setting as primary, unset other primary addresses
            if (request.IsPrimary)
            {
                var existingAddresses = await _addressRepository.GetByUserIdAsync(request.UserId, cancellationToken);
                foreach (var addr in existingAddresses.Where(a => a.IsPrimary))
                {
                    addr.IsPrimary = false;
                    addr.UpdatedAt = DateTime.UtcNow;
                    _addressRepository.Update(addr);
                }
            }

            var address = new Domain.Entities.UserAddress
            {
                UserId = request.UserId,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                City = request.City,
                State = request.State,
                PostalCode = request.PostalCode,
                Country = request.Country,
                IsPrimary = request.IsPrimary
            };

            await _addressRepository.AddAsync(address, cancellationToken);
            await _addressRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Address created for user {UserId}", request.UserId);

            var response = new AddUserAddressResponse(
                address.Id,
                "Address added successfully");

            return ApiResponse<AddUserAddressResponse>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding address for user {UserId}", request.UserId);
            return ApiResponse<AddUserAddressResponse>.FailResponse(
                "An error occurred while adding address",
                new List<string> { ex.Message });
        }
    }
}
