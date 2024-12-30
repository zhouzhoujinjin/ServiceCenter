using PureCode.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureCode.Core.UserQueryFilters;

public abstract class UserQueryFilterBase : IUserQueryFilter
{
  public virtual IEnumerable<Type> IgnoreFilters => Array.Empty<Type>();

  public virtual IQueryable<UserEntity> QueryFilter(IQueryable<UserEntity> userQuery,
    Dictionary<string, object?> conditions)
  {
    return userQuery;
  }
}