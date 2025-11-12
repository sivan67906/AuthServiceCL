using AuthService.API.Constants;
using AuthService.Application.Common;
using AuthService.Application.Features.TwoFactor.Enable;
using AuthService.Application.Features.TwoFactor.Verify;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing;
using System.Security.Claims;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.AdminOrUser)]
public sealed class TwoFactorController : ControllerBase
{
    private readonly IMediator _mediator;

    public TwoFactorController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("enable")]
    public async Task<ActionResult<ApiResponse<Enable2FAResponse>>> Enable2FA()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var command = new Enable2FACommand(userId);
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("verify")]
    public async Task<ActionResult<ApiResponse<Verify2FAResponse>>> Verify2FA(Verify2FACommand command)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim != command.UserId.ToString())
        {
            return Forbid();
        }

        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("qrcode")]
    public IActionResult GetQRCode([FromQuery] string qrCodeUri)
    {
        if (string.IsNullOrEmpty(qrCodeUri))
        {
            return BadRequest("QR code URI is required");
        }

        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(qrCodeUri, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeImage = qrCode.GetGraphic(20);

        return File(qrCodeImage, "image/png");
    }
}
