using PureCode.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PureCode.Core.UserQueryFilters;

public class UserNameFilter : UserQueryFilterBase
{
  public override IQueryable<UserEntity> QueryFilter(IQueryable<UserEntity> userQuery,
    Dictionary<string, object?> conditions)
  {
    if (conditions.ContainsKey("userName"))
    {
      var userName = conditions["userName"].ToString()!.ToUpper();
      var query = userQuery.Expression;
      return userQuery.Where(x => x.NormalizedUserName!.StartsWith(userName));
    }

    return userQuery;
  }
}