using AuthService.Domain.Common;

namespace AuthService.Domain.Entities;

public sealed class UserAddress : BaseEntity
{
    public Guid UserId { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsPrimary { get; set; } = false;
    
    // Navigation property
    public ApplicationUser User { get; set; } = null!;
}
