using AuthService.Application.Common;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace AuthService.Application.Features.Auth.Login;

public sealed class LoginHandler : IRequestHandler<LoginCommand, ApiResponse<LoginResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly ILogger<LoginHandler> _logger;

    public LoginHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService,
        ILogger<LoginHandler> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<ApiResponse<LoginResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return ApiResponse<LoginResponse>.FailResponse(
                    "Invalid email or password",
                    new List<string> { "Authentication failed" });
            }

            if (!user.IsActive)
            {
                return ApiResponse<LoginResponse>.FailResponse(
                    "Account is deactivated",
                    new List<string> { "Account inactive" });
            }

            if (!user.EmailConfirmed)
            {
                return ApiResponse<LoginResponse>.FailResponse(
                    "Email not confirmed",
                    new List<string> { "Please confirm your email before logging in" });
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                request.Password,
                request.RememberMe,
                lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                return ApiResponse<LoginResponse>.FailResponse(
                    "Account is locked",
                    new List<string> { "Too many failed attempts" });
            }

            if (result.RequiresTwoFactor)
            {
                return ApiResponse<LoginResponse>.FailResponse(
                    "Two-factor authentication required",
                    new List<string> { "2FA_REQUIRED" });
            }

            if (!result.Succeeded)
            {
                return ApiResponse<LoginResponse>.FailResponse(
                    "Invalid email or password",
                    new List<string> { "Authentication failed" });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email!),
                new(ClaimTypes.Name, user.FullName),
                new("UserId", user.Id.ToString())
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var accessToken = _jwtService.GenerateAccessToken(claims);
            var refreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("User {Email} logged in successfully", user.Email);

            var userDto = new UserDto(
                user.Id,
                user.Email!,
                user.FirstName,
                user.LastName,
                user.PhoneNumber,
                roles);

            var response = new LoginResponse(
                accessToken,
                refreshToken,
                _jwtService.GetTokenExpiryTime(),
                userDto);

            return ApiResponse<LoginResponse>.SuccessResponse(response, "Login successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", request.Email);
            return ApiResponse<LoginResponse>.FailResponse(
                "An error occurred during login",
                new List<string> { ex.Message });
        }
    }
}
