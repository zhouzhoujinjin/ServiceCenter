using Approval.Abstracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Approval.Hooks
{
  public static class ServiceCollectionExtensions
  {
    public static void AddApprovalHooks(this IServiceCollection services, IConfiguration configuration)
    {
      services.Configure<LicenseServiceOptions>(configuration.GetSection(nameof(LicenseServiceOptions)));
      services.AddScoped<IApprovalHook, LicenseRequestHook>();
    }
  }
}
