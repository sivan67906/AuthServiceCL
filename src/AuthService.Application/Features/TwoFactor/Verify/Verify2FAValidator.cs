using FluentValidation;

namespace AuthService.Application.Features.TwoFactor.Verify;

public sealed class Verify2FAValidator : AbstractValidator<Verify2FACommand>
{
    public Verify2FAValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Verification code is required")
            .Length(6).WithMessage("Code must be 6 digits")
            .Matches(@"^\d{6}$").WithMessage("Code must contain only digits");
    }
}
