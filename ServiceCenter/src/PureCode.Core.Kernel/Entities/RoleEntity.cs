using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace PureCode.Core.Entities
{
  public class RoleEntity : IdentityRole<ulong>
  {
    public RoleEntity()
    {
      UserRoles = new List<UserRoleEntity>();
      RoleClaims = new List<RoleClaimEntity>();
    }

    public string Title { get; set; } = string.Empty;

    public ICollection<UserRoleEntity> UserRoles { get; set; }
    public ICollection<RoleClaimEntity> RoleClaims { get; set; }
  }

  public class UserRoleEntity : IdentityUserRole<ulong>
  {
    public UserEntity? User { get; set; }
    public RoleEntity? Role { get; set; }
  }
}