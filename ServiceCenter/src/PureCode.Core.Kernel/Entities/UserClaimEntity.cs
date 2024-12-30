using Microsoft.AspNetCore.Identity;

namespace PureCode.Core.Entities
{
  public class UserClaimEntity : IdentityUserClaim<ulong>
  {
    public new ulong Id { get; set; }
    public UserEntity? User { get; set; }
  }
}