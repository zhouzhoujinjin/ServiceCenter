using Microsoft.EntityFrameworkCore;
using PureCode.Core.Entities;
using PureCode.Core.Kernel;
using PureCode.Core.UserQueryFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureCode.Core.ProfileFeature.UserQueryFilters;

public class SearchableProfileFilter(PureCodeDbContext context, ProfileKeyMap keyMap) : UserQueryFilterBase
{
  private readonly DbSet<UserClaimEntity> _userClaimSet = context.Set<UserClaimEntity>();

  public override IQueryable<UserEntity> QueryFilter(IQueryable<UserEntity> userQuery,
    Dictionary<string, object?> conditions)
  {
    if (conditions.TryGetValue("any", out var condition) && condition != null)
    {
      var search = condition.ToString()!;
      var searchableKeys = keyMap.GetSearchableKeyNames().Select(x => $"{ProfileKeys.ClaimTypePrefix}:{x}");
      var ups = _userClaimSet.Where(up =>
          searchableKeys.Contains(up.ClaimType) && up.ClaimValue != null && up.ClaimValue.StartsWith(search)).Distinct()
        .Select(up => up.UserId);
      return userQuery.Where(u => ups.Contains(u.Id));
    }

    return userQuery;
  }
}