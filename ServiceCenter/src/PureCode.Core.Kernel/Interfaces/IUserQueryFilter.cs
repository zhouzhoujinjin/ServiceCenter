using PureCode.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureCode.Core;

public interface IUserQueryFilter
{
  IEnumerable<Type> IgnoreFilters { get; }

  IQueryable<UserEntity> QueryFilter(IQueryable<UserEntity> userQuery, Dictionary<string, object?> conditions);
}