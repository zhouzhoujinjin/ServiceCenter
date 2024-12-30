using Microsoft.EntityFrameworkCore;
using PureCode.Core.Entities;
using PureCode.Core.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureCode.Core.UserQueryFilters;

public class NameFilter : UserQueryFilterBase
{
  private readonly DbSet<UserClaimEntity> userClaimSet;

  public NameFilter(PureCodeDbContext context)
  {
    userClaimSet = context.Set<UserClaimEntity>();
  }

  public override IEnumerable<Type> IgnoreFilters => new Type[] { typeof(UserNameFilter) };

  public override IQueryable<UserEntity> QueryFilter(IQueryable<UserEntity> userQuery,
    Dictionary<string, object?> conditions)
  {
    if (conditions.ContainsKey("name") && conditions["name"] != null)
    {
      var search = conditions["name"].ToString()!;
      var fullNameKeyName = $"{ProfileKeys.ClaimTypePrefix}:{ProfileKeys.FullName}";
      var pinyinKeyName = $"{ProfileKeys.ClaimTypePrefix}:{ProfileKeys.PinYin}";
      var ups = userClaimSet
        .Where(up =>
          up.ClaimValue != null &&
          ((up.ClaimType == fullNameKeyName && up.ClaimValue.StartsWith(search)) ||
           (up.ClaimType == pinyinKeyName && up.ClaimValue.StartsWith(search))))
        .Distinct().Select(up => up.UserId);
      return userQuery.Where(u => u.NormalizedUserName!.StartsWith(search.ToUpper()) || ups.Contains(u.Id));
    }

    return userQuery;
  }
}