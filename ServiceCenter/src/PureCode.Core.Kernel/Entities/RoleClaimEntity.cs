using Microsoft.AspNetCore.Identity;

namespace PureCode.Core.Entities
{
  public class RoleClaimEntity : IdentityRoleClaim<ulong>
  {
    public new ulong Id { get; set; }
    public RoleEntity? Role { get; set; }
  }
}