using AuthService.Application.Features.UserAddress.UpdateAddress;
using FluentValidation;

namespace AuthService.Application.Features.Auth.UserAddress.UpdateAddress;

public sealed class UpdateUserAddressValidator : AbstractValidator<UpdateUserAddressCommand>
{
    public UpdateUserAddressValidator()
    {
        RuleFor(x => x.AddressLine1)
            .NotEmpty().WithMessage("Address line 1 is required")
            .MaximumLength(200).WithMessage("Address line 1 must not exceed 200 characters");

        RuleFor(x => x.AddressLine2)
            .MaximumLength(200).WithMessage("Address line 2 must not exceed 200 characters");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City must not exceed 100 characters");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required")
            .MaximumLength(100).WithMessage("State must not exceed 100 characters");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Postal code is required")
            .MaximumLength(20).WithMessage("Postal code must not exceed 20 characters");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required")
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters");
    }
}
