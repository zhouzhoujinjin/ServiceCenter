using PureCode.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureCode.Core.UserQueryFilters;

public class RoleNameFilter : UserQueryFilterBase
{
  public override IQueryable<UserEntity> QueryFilter(IQueryable<UserEntity> userQuery,
    Dictionary<string, object?> conditions)
  {
    if (conditions.ContainsKey("role"))
    {
      var roles = conditions["role"].ToString()!.Split(",").Select(x => x.ToUpper());
      return userQuery.Where(u => u.UserRoles.Select(x => x.Role!.NormalizedName).Intersect(roles).Any());
    }

    return userQuery;
  }
}