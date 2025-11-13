using AuthService.Application.Common;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace AuthService.Application.Features.Auth.ExternalLogin;

public sealed class ExternalLoginHandler : IRequestHandler<ExternalLoginCommand, ApiResponse<ExternalLoginResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IExternalLoginRepository _externalLoginRepository;
    private readonly IJwtService _jwtService;
    private readonly ILogger<ExternalLoginHandler> _logger;

    public ExternalLoginHandler(
        UserManager<ApplicationUser> userManager,
        IExternalLoginRepository externalLoginRepository,
        IJwtService jwtService,
        ILogger<ExternalLoginHandler> logger)
    {
        _userManager = userManager;
        _externalLoginRepository = externalLoginRepository;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<ApiResponse<ExternalLoginResponse>> Handle(
        ExternalLoginCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Check if external login already exists
            var externalLogin = await _externalLoginRepository.GetByProviderKeyAsync(
                request.Provider,
                request.ProviderKey,
                cancellationToken);

            ApplicationUser? user;

            if (externalLogin != null)
            {
                // User exists, log them in
                user = externalLogin.User;
            }
            else
            {
                // Check if user exists by email
                user = await _userManager.FindByEmailAsync(request.Email);

                if (user == null)
                {
                    // Create new user
                    user = new ApplicationUser
                    {
                        UserName = request.Email,
                        Email = request.Email,
                        FirstName = request.FirstName ?? string.Empty,
                        LastName = request.LastName ?? string.Empty,
                        EmailConfirmed = true, // External providers verify email
                        IsActive = true
                    };

                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                    {
                        var errors = createResult.Errors.Select(e => e.Description).ToList();
                        return ApiResponse<ExternalLoginResponse>.FailResponse(
                            "Failed to create user",
                            errors);
                    }

                    await _userManager.AddToRoleAsync(user, "User");
                }

                // Create external login link
                var newExternalLogin = new Domain.Entities.ExternalLogin
                {
                    UserId = user.Id,
                    Provider = request.Provider,
                    ProviderKey = request.ProviderKey,
                    ProviderDisplayName = request.Provider.ToString()
                };

                await _externalLoginRepository.AddAsync(newExternalLogin, cancellationToken);
                await _externalLoginRepository.SaveChangesAsync(cancellationToken);
            }

            if (!user.IsActive)
            {
                return ApiResponse<ExternalLoginResponse>.FailResponse(
                    "Account is deactivated",
                    new List<string> { "Account inactive" });
            }

            // Generate tokens
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

            _logger.LogInformation("User {Email} logged in via {Provider}", user.Email, request.Provider);

            var response = new ExternalLoginResponse(
                accessToken,
                refreshToken,
                _jwtService.GetTokenExpiryTime(),
                new UserDto(
                    user.Id,
                    user.Email!,
                    user.FirstName,
                    user.LastName,
                    user.PhoneNumber,
                    roles));

            return ApiResponse<ExternalLoginResponse>.SuccessResponse(response, "Login successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during external login for {Email}", request.Email);
            return ApiResponse<ExternalLoginResponse>.FailResponse(
                "An error occurred during external login",
                new List<string> { ex.Message });
        }
    }
}
