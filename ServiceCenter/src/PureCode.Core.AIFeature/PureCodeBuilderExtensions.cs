using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureCode.Core.AIFeature
{
  public static class PureCodeBuilderExtensions
  {
    public static PureCodeServiceCollectionBuilder WithAI(this PureCodeServiceCollectionBuilder builder)
    {
      builder.Services.AddScoped<AIManager>();
      //builder.AddTreeType(DepartmentManager.TreeType);
      return builder;
    }
  }
}
