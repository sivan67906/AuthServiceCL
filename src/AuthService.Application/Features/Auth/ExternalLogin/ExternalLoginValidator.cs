using FluentValidation;

namespace AuthService.Application.Features.Auth.ExternalLogin;

public sealed class ExternalLoginValidator : AbstractValidator<ExternalLoginCommand>
{
    public ExternalLoginValidator()
    {
        RuleFor(x => x.ProviderKey)
            .NotEmpty().WithMessage("Provider key is required");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Provider)
            .IsInEnum().WithMessage("Invalid provider");
    }
}