using AuthService.Domain.Entities;
using AuthService.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistence;

public sealed class CommandDbContext : IdentityDbContext<
    ApplicationUser,
    ApplicationRole,
    Guid,
    IdentityUserClaim<Guid>,
    ApplicationUserRole,
    IdentityUserLogin<Guid>,
    IdentityRoleClaim<Guid>,
    IdentityUserToken<Guid>>
{
    public CommandDbContext(DbContextOptions<CommandDbContext> options) : base(options)
    {
    }

    public DbSet<UserAddress> UserAddresses => Set<UserAddress>();
    public DbSet<ExternalLogin> ExternalLogins => Set<ExternalLogin>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("ApplicationUsers");
        });

        builder.Entity<ApplicationRole>(entity =>
        {
            entity.ToTable("ApplicationRoles");
        });

        builder.Entity<ApplicationUserRole>(entity =>
        {
            entity.ToTable("ApplicationUserRoles");
            
            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<IdentityUserClaim<Guid>>(entity =>
        {
            entity.ToTable("ApplicationUserClaims");
        });

        builder.Entity<IdentityUserLogin<Guid>>(entity =>
        {
            entity.ToTable("ApplicationUserLogins");
        });

        builder.Entity<IdentityRoleClaim<Guid>>(entity =>
        {
            entity.ToTable("ApplicationRoleClaims");
        });

        builder.Entity<IdentityUserToken<Guid>>(entity =>
        {
            entity.ToTable("ApplicationUserTokens");
        });

        builder.ApplyConfigurationsFromAssembly(typeof(CommandDbContext).Assembly);

        SeedData(builder);
    }

    private static void SeedData(ModelBuilder builder)
    {
        var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var userRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var adminUserId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        builder.Entity<ApplicationRole>().HasData(
            new ApplicationRole
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                Description = "Administrator role with full access",
                CreatedAt = new DateTime(2023, 01, 01),
                ConcurrencyStamp = "3f9c1e3d-8b7a-4e6f-9c2e-2a1f4d8e9b6c"
            },
            new ApplicationRole
            {
                Id = userRoleId,
                Name = "User",
                NormalizedName = "USER",
                Description = "Standard user role",
                CreatedAt = new DateTime(2023, 01, 01),
                ConcurrencyStamp = "7a2d4f1c-3e8b-4c9f-bd3a-5e2c9a7f1d4e"
            }
        );

        //var passwordHasher = new PasswordHasher<ApplicationUser>();
        var adminUser = new ApplicationUser
        {
            Id = adminUserId,
            UserName = "admin@authservice.com",
            NormalizedUserName = "ADMIN@AUTHSERVICE.COM",
            Email = "admin@authservice.com",
            NormalizedEmail = "ADMIN@AUTHSERVICE.COM",
            EmailConfirmed = true,
            FirstName = "System",
            LastName = "Administrator",
            IsActive = true,
            CreatedAt = new DateTime(2023, 01, 05),
            SecurityStamp = "b6f3a9d2-1c4e-4b8f-9a7e-3d2f1c8b6a9e",
            ConcurrencyStamp = "9e1f3d2c-7b4a-4c8e-bd2f-1a3e9f7c6d5b"
        };
        //adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin@123456");
        adminUser.PasswordHash = "AQAAAAIAAYagAAAAELv8Xc55AlNmj7R5Wh1nMwu7YYwb9j/6j6FEDOpRgw33EkEK6xeNZyM5tWcKQRCMoA==";


        builder.Entity<ApplicationUser>().HasData(adminUser);

        builder.Entity<ApplicationUserRole>().HasData(
            new ApplicationUserRole
            {
                UserId = adminUserId,
                RoleId = adminRoleId
            }
        );
    }
}
