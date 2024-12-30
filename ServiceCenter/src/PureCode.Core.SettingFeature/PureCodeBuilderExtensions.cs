using Microsoft.Extensions.DependencyInjection;
using PureCode.Core.Managers;

namespace PureCode.Core.SettingFeature
{
  public static class PureCodeBuilderExtensions
  {
    public static PureCodeServiceCollectionBuilder WithSetting(this PureCodeServiceCollectionBuilder builder )
    {
      builder.Services.AddScoped<SettingManager>();      
      return builder;
    }
  }
}