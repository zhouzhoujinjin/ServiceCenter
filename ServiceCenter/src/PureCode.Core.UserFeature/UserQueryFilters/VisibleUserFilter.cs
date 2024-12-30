using PureCode.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureCode.Core.UserQueryFilters;

public class VisibleUserFilter : UserQueryFilterBase
{
  public override IQueryable<UserEntity> QueryFilter(IQueryable<UserEntity> userQuery,
    Dictionary<string, object?> conditions)
  {
    if (conditions.ContainsKey("visible"))
    {
      var visible = (bool)conditions["visible"];
      return userQuery.Where(x => x.IsVisible == visible);
    }
    else
    {
      return userQuery.Where(x => x.IsVisible == true);
    }
  }
}