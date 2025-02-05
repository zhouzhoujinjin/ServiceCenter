using Approval.Managers;

namespace Approval
{
  public static class ServiceCollectionExtensions
  {
    public static void AddApproval(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddScoped<ConfigManager>();
    }
  }
}
