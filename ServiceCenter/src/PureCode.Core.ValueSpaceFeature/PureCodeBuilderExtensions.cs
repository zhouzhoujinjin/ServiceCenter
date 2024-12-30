using Microsoft.Extensions.DependencyInjection;
using PureCode.Core.Converters;
using PureCode.Core.Managers;

namespace PureCode.Core.ValueSpaceFeature
{
  public static class PureCodeBuilderExtensions
  {
    public static PureCodeServiceCollectionBuilder WithValueSpace(this PureCodeServiceCollectionBuilder builder)
    {
      builder.Services.AddSingleton(sp => ValueSpaceManager.ValueSpaceMap);
      builder.Services.AddScoped<ValueSpaceManager>();
      builder.MvcBuilder.AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.Converters.Add(new JsonValueSpaceConverter());
      });
      return builder;
    }
  }

  public static class PureCodeApplicationBuilderExtensions
  {
    public static PureCodeApplicationBuilder UseValueSpace(this PureCodeApplicationBuilder builder)
    {
      using var scope = builder.App.ApplicationServices.CreateScope();
      var valueSpaceManager = scope.ServiceProvider.GetService<ValueSpaceManager>();
      valueSpaceManager?.InitializeAsync().Wait();
      return builder;
    }
  }
}