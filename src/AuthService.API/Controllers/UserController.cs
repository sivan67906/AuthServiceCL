using AuthService.API.Constants;
using AuthService.Application.Common;
using AuthService.Application.Features.User.GetProfile;
using AuthService.Application.Features.User.UpdateProfile;
using AuthService.Application.Features.UserAddress.AddAddress;
using AuthService.Application.Features.UserAddress.DeleteAddress;
using AuthService.Application.Features.UserAddress.GetAddresses;
using AuthService.Application.Features.UserAddress.UpdateAddress;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.AdminOrUser)]
public sealed class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("profile")]
    public async Task<ActionResult<ApiResponse<ProfileDto>>> GetProfile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var query = new GetProfileQuery(userId);
        var result = await _mediator.Send(query);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPut("profile")]
    public async Task<ActionResult<ApiResponse<UpdateProfileResponse>>> UpdateProfile(UpdateProfileCommand command)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim != command.UserId.ToString())
        {
            return Forbid();
        }

        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<GetUserAddressesResponse>>> GetAddresses()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var query = new GetUserAddressesQuery(userId);
        var result = await _mediator.Send(query);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<AddUserAddressResponse>>> AddAddress(AddUserAddressCommand command)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim != command.UserId.ToString())
        {
            return Forbid();
        }

        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{addressId}")]
    public async Task<ActionResult<ApiResponse<UpdateUserAddressResponse>>> UpdateAddress(
        Guid addressId,
        UpdateUserAddressCommand command)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim != command.UserId.ToString())
        {
            return Forbid();
        }

        if (addressId != command.AddressId)
        {
            return BadRequest("Address ID mismatch");
        }

        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{addressId}")]
    public async Task<ActionResult<ApiResponse<DeleteUserAddressResponse>>> DeleteAddress(Guid addressId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var command = new DeleteUserAddressCommand(addressId, userId);
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
