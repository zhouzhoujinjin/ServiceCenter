using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace PureCode.Core.Entities
{
  public class UserEntity : IdentityUser<ulong>, IEntity, IHasCreationTime, IHasDeletionTime
  {
    public bool IsDeleted { get; set; }
    public bool IsVisible { get; set; }

    public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
    public ICollection<UserClaimEntity> UserClaims { get; set; } = new List<UserClaimEntity>();
    public DateTime CreatedTime { get; set; }
    public DateTime? DeletedTime { get; set; }
  }
}