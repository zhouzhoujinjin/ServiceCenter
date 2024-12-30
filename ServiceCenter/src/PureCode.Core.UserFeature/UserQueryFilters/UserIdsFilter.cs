using PureCode.Core.Entities;
using PureCode.Core.UserQueryFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureCode.Core.UserFeature.UserQueryFilters;

public class UserIdsFilter : UserQueryFilterBase
{
  public override IQueryable<UserEntity> QueryFilter(IQueryable<UserEntity> userQuery,
    Dictionary<string, object?> conditions)
  {
    if (conditions.ContainsKey("userIds"))
    {
      var userIds = conditions["userIds"].ToString()!.Split(",").Select(x => ulong.Parse(x)).ToList();
      if (userIds.Any())
      {
        return userQuery.Where(u => userIds.Contains(u.Id));
      }
    }

    return userQuery;
  }
}