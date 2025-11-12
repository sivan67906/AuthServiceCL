using AuthService.Domain.Common;
using AuthService.Domain.Enums;

namespace AuthService.Domain.Entities;

public sealed class ExternalLogin : BaseEntity
{
    public Guid UserId { get; set; }
    public ExternalProvider Provider { get; set; }
    public string ProviderKey { get; set; } = string.Empty;
    public string? ProviderDisplayName { get; set; }
    
    // Navigation property
    public ApplicationUser User { get; set; } = null!;
}
