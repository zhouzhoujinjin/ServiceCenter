using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PureCode.Core.Entities;
using PureCode.Core.Utils;

namespace PureCode.Core.ModelCreations
{
  public class UserModelCreation : IModelCreation
  {
    public int Seq => (int)ModelCreatingPriority.Level1;

    public static void OnModelCreating(ModelBuilder builder, string? dbType = null)
    {
      builder.Entity<IdentityUserLogin<ulong>>(entity =>
      {
        entity.ToTable("Core_UserLogin");
        entity.Property(e => e.UserId).HasColumnName("UserId");
      });
      builder.Entity<IdentityUserToken<ulong>>(entity =>
      {
        entity.ToTable("Core_UserToken");
        entity.Property(e => e.UserId).HasColumnName("UserId");
      });

      builder.Entity<RoleEntity>(entity =>
      {
        entity.ToTable("Core_Role").HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.HasMany(e => e.RoleClaims).WithOne(e => e.Role).HasForeignKey(e => e.RoleId);
        entity.HasMany(e => e.UserRoles).WithOne(e => e.Role).HasForeignKey(c => c.RoleId);
        if (dbType == "oracle")
        {
          entity.Property(e => e.Id).HasValueGenerator((_, __) => new ULongSequenceValueGenerator("S_CORE_ROLE"));
        }
      });
      builder.Entity<RoleClaimEntity>(entity =>
      {
        entity.ToTable("Core_RoleClaim").HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.Property(e => e.RoleId).HasColumnName("RoleId");
        if (dbType == "oracle")
        {
          entity.Property(e => e.Id).HasValueGenerator((_, __) => new LongSequenceValueGenerator("S_CORE_ROLECLAIM"));
        }
      });
      builder.Entity<UserEntity>(entity =>
      {
        entity.ToTable("Core_User");
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.HasMany(u => u.UserClaims).WithOne(u => u.User).HasForeignKey(c => c.UserId);
        entity.HasMany(u => u.UserRoles).WithOne(e => e.User).HasForeignKey(c => c.UserId);
        if (dbType == "oracle")
        {
          entity.Property(e => e.Id).HasValueGenerator((_, __) => new LongSequenceValueGenerator("S_CORE_USER"));
        }
      });
      builder.Entity<UserClaimEntity>(entity =>
      {
        entity.ToTable("Core_UserClaim");
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.Property(e => e.UserId).HasColumnName("UserId");
        if (dbType == "oracle")
        {
          entity.Property(e => e.Id).HasValueGenerator((_, __) => new LongSequenceValueGenerator("S_CORE_USERCLAIM"));
        }
      });
      builder.Entity<UserRoleEntity>(entity =>
      {
        entity.ToTable("Core_UserRole").HasKey(e => new { e.UserId, e.RoleId });
        entity.HasOne(userRole => userRole.Role).WithMany(role => role.UserRoles)
                  .HasForeignKey(userRole => userRole.RoleId);
        entity.HasOne(userRole => userRole.User).WithMany(user => user.UserRoles)
                  .HasForeignKey(userRole => userRole.UserId);
      });
    }
  }
}