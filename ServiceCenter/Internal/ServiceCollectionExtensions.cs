using Internal.Managers;

namespace Internal
{
  public static class ServiceCollectionExtensions
  {
    public static void AddInternal(this IServiceCollection services)
    {
      services.AddScoped<ConfigManager>();
    }
  }
}
