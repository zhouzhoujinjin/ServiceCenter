using Microsoft.Extensions.DependencyInjection;
using PureCode.Core.Managers;
using PureCode.Core.TreeFeature;

namespace PureCode.Core.MenuFeature
{
  public static class PureCodeBuilderExtensions
  {
    public static PureCodeServiceCollectionBuilder WithNavMenu(this PureCodeServiceCollectionBuilder builder)
    {
      builder.Services.AddScoped<MenuManager>();
      builder.AddTreeType(MenuManager.TreeType);
      return builder;
    }
  }
}