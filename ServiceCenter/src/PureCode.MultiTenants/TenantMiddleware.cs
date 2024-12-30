using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PureCode.MultiTenants
{
  public class TenantMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly TenantsContainer _container;
    private readonly TenantOptions _options;

    public TenantMiddleware(RequestDelegate next, TenantsContainer container, IOptions<TenantOptions> tenantOptions)
    {
      _next = next;
      _container = container;
      _options = tenantOptions.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      TenantContext tenant = null;
      switch (_options.DiscoveryType)
      {
        case TenantDiscoveryType.Domain:
          tenant = _container.GetContext(context.Request.Host.Value);
          break;

        case TenantDiscoveryType.Header:
          if (!string.IsNullOrEmpty(_options.HeaderKey) && context.Request.Headers.TryGetValue(_options.HeaderKey, out var id))
          {
            tenant = _container.GetContext(id);
          }
          break;

        case TenantDiscoveryType.RefererUrl:
          if (context.Request.Headers.TryGetValue("Referer", out var referer))
          {
            var regex = new Regex(_options.RefererRegexPattern);
            var match = regex.Match(referer);
            if (match.Success && match.Groups.Count == 2)
            {
              tenant = _container.GetContext(match.Groups[1].Value);
            }
          }
          break;
      }

      if (tenant != null)
      {
        context.Items.Add("TenantContext", tenant);
      }
      await _next(context);
    }
  }
}