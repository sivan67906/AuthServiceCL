using AuthService.Application.Common;
using AuthService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.UserAddress.DeleteAddress;

public sealed class DeleteUserAddressHandler : IRequestHandler<DeleteUserAddressCommand, ApiResponse<DeleteUserAddressResponse>>
{
    private readonly IUserAddressRepository _addressRepository;
    private readonly ILogger<DeleteUserAddressHandler> _logger;

    public DeleteUserAddressHandler(
        IUserAddressRepository addressRepository,
        ILogger<DeleteUserAddressHandler> logger)
    {
        _addressRepository = addressRepository;
        _logger = logger;
    }

    public async Task<ApiResponse<DeleteUserAddressResponse>> Handle(
        DeleteUserAddressCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var address = await _addressRepository.GetByIdAsync(request.AddressId, cancellationToken);
            if (address == null)
            {
                return ApiResponse<DeleteUserAddressResponse>.FailResponse(
                    "Address not found",
                    new List<string> { "Invalid address ID" });
            }

            if (address.UserId != request.UserId)
            {
                return ApiResponse<DeleteUserAddressResponse>.FailResponse(
                    "Unauthorized",
                    new List<string> { "You don't have permission to delete this address" });
            }

            _addressRepository.Remove(address);
            await _addressRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Address {AddressId} deleted for user {UserId}", request.AddressId, request.UserId);

            var response = new DeleteUserAddressResponse("Address deleted successfully");
            return ApiResponse<DeleteUserAddressResponse>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting address {AddressId} for user {UserId}", request.AddressId, request.UserId);
            return ApiResponse<DeleteUserAddressResponse>.FailResponse(
                "An error occurred while deleting address",
                new List<string> { ex.Message });
        }
    }
}
