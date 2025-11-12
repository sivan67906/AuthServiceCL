using AuthService.Application.Common;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Auth.Register;

public sealed class RegisterHandler : IRequestHandler<RegisterCommand, ApiResponse<RegisterResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<RegisterHandler> _logger;

    public RegisterHandler(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        ILogger<RegisterHandler> logger)
    {
        _userManager = userManager;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<ApiResponse<RegisterResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return ApiResponse<RegisterResponse>.FailResponse(
                    "User with this email already exists",
                    new List<string> { "Email already registered" });
            }

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                EmailConfirmed = false,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponse<RegisterResponse>.FailResponse(
                    "Failed to create user",
                    errors);
            }

            await _userManager.AddToRoleAsync(user, "User");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"https://localhost:5001/api/auth/verify-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            
            await _emailService.SendConfirmationEmailAsync(user.Email!, confirmationLink, cancellationToken);

            _logger.LogInformation("User {Email} registered successfully", user.Email);

            var response = new RegisterResponse(
                user.Id,
                user.Email!,
                user.FirstName,
                user.LastName,
                "Registration successful. Please check your email to confirm your account.");

            return ApiResponse<RegisterResponse>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration for {Email}", request.Email);
            return ApiResponse<RegisterResponse>.FailResponse(
                "An error occurred during registration",
                new List<string> { ex.Message });
        }
    }
}
