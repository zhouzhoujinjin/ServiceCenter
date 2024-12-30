using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;

namespace PureCode.MultiTenants
{
  public static class ServiceCollectionExtensions
  {
    public static TenantBuilder AddTenants(
        this IServiceCollection services,
        IConfigurationSection config
    )
    {
      return services.AddTenants(config, null);
    }

    public static TenantBuilder AddTenants(
        this IServiceCollection services,
        IConfigurationSection config,
        Action<TenantOptions> setupAction)
    {
      services.Configure<TenantOptions>(config);
      services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
      services.AddSingleton<TenantsContainer>();
      services.AddMemoryCache();

      if (setupAction != null)
      {
        services.Configure(setupAction);
      }

      var builder = new TenantBuilder(services);

      return builder;
    }

    public static void UseTenants(this IApplicationBuilder app)
    {
      app.UseMiddleware<TenantMiddleware>();
      var container = app.ApplicationServices.GetService<TenantsContainer>();
      var optionsDelegate = app.ApplicationServices.GetService<IOptionsMonitor<TenantOptions>>();
      var optionsAccessor = app.ApplicationServices.GetService<IOptions<TenantOptions>>();
      optionsAccessor.Value.Tenants.ForEach(t => container.Register(t));
      optionsDelegate.OnChange((options) =>
      {
        container.Clear();
        options.Tenants.ForEach(o =>
        {
          container.Register(o);
        });
      });
    }
  }
}