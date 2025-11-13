using AuthService.Application.Common;
using AuthService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.UserAddress.UpdateAddress;

public sealed class UpdateUserAddressHandler : IRequestHandler<UpdateUserAddressCommand, ApiResponse<UpdateUserAddressResponse>>
{
    private readonly IUserAddressRepository _addressRepository;
    private readonly ILogger<UpdateUserAddressHandler> _logger;

    public UpdateUserAddressHandler(
        IUserAddressRepository addressRepository,
        ILogger<UpdateUserAddressHandler> logger)
    {
        _addressRepository = addressRepository;
        _logger = logger;
    }

    public async Task<ApiResponse<UpdateUserAddressResponse>> Handle(
        UpdateUserAddressCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var address = await _addressRepository.GetByIdAsync(request.AddressId, cancellationToken);
            if (address == null)
            {
                return ApiResponse<UpdateUserAddressResponse>.FailResponse(
                    "Address not found",
                    new List<string> { "Invalid address ID" });
            }

            if (address.UserId != request.UserId)
            {
                return ApiResponse<UpdateUserAddressResponse>.FailResponse(
                    "Unauthorized",
                    new List<string> { "You don't have permission to update this address" });
            }

            // If setting as primary, unset other primary addresses
            if (request.IsPrimary && !address.IsPrimary)
            {
                var existingAddresses = await _addressRepository.GetByUserIdAsync(request.UserId, cancellationToken);
                foreach (var addr in existingAddresses.Where(a => a.IsPrimary && a.Id != address.Id))
                {
                    addr.IsPrimary = false;
                    addr.UpdatedAt = DateTime.UtcNow;
                    _addressRepository.Update(addr);
                }
            }

            address.AddressLine1 = request.AddressLine1;
            address.AddressLine2 = request.AddressLine2;
            address.City = request.City;
            address.State = request.State;
            address.PostalCode = request.PostalCode;
            address.Country = request.Country;
            address.IsPrimary = request.IsPrimary;
            address.UpdatedAt = DateTime.UtcNow;

            _addressRepository.Update(address);
            await _addressRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Address {AddressId} updated for user {UserId}", request.AddressId, request.UserId);

            var response = new UpdateUserAddressResponse("Address updated successfully");
            return ApiResponse<UpdateUserAddressResponse>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating address {AddressId} for user {UserId}", request.AddressId, request.UserId);
            return ApiResponse<UpdateUserAddressResponse>.FailResponse(
                "An error occurred while updating address",
                new List<string> { ex.Message });
        }
    }
}
