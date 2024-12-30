using Microsoft.Extensions.DependencyInjection;
using PureCode.Core.TreeFeature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureCode.Core.DepartmentFeature
{
  public static class PureCodeBuilderExtensions
  {
    public static PureCodeServiceCollectionBuilder WithDepartment(this PureCodeServiceCollectionBuilder builder)
    {
      builder.Services.AddScoped<DepartmentManager>();
      builder.AddTreeType(DepartmentManager.TreeType);
      return builder;
    }
  }
}
