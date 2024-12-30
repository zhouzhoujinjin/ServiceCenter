using Microsoft.Extensions.DependencyInjection;

namespace PureCode.MultiTenants
{
  public class TenantBuilder
  {
    /// <summary>
    /// The services being configured.
    /// </summary>
    public IServiceCollection Services { get; }

    public TenantBuilder(IServiceCollection services)
    {
      Services = services;
    }
  }
}