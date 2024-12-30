using PureCode.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureCode.Core.UserQueryFilters;

public class DeletedUserFilter : UserQueryFilterBase
{
  public override IQueryable<UserEntity> QueryFilter(IQueryable<UserEntity> userQuery,
    Dictionary<string, object?> conditions)
  {
    if (conditions.ContainsKey("deleted"))
    {
      var deleted = (bool)conditions["deleted"];
      return userQuery.Where(x => x.IsDeleted == deleted);
    }
    else
    {
      return userQuery.Where(x => x.IsDeleted == false);
    }
  }
}