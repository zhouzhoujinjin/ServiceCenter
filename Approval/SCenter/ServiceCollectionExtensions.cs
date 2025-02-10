using SCenter.Managers;
using SCenter.Services;

namespace SCenter
{
  public static class ServiceCollectionExtensions
  {
    public static void AddSCenter(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddScoped<ConfigManager>();
      services.AddScoped<PdfService>();
    }
  }
}
