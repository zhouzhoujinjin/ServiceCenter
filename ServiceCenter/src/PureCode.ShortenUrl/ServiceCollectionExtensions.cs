using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PureCode.ShortenUrl
{
  public static class ServiceCollectionExtensions
  {
    public static void AddShortenUrl(this IServiceCollection services, IConfigurationSection config)
    {
      services.AddSingleton<ShortenUrlApi>();
      services.AddSingleton<ShortenUrlClient>();
      services.Configure<ShortenUrlOptions>(config);
    }
  }
}