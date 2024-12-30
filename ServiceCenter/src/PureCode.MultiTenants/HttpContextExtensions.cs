using Microsoft.AspNetCore.Http;

namespace PureCode.MultiTenants
{
  public static class HttpContextExtensions
  {
    public static TenantContext GetTenant(this HttpContext ctx)
    {
      //ctx.Items.TryGetValue("TenantContext", out var tenant);
      //return tenant as TenantContext;
      return new TenantContext
      {
        Id = "1",
        Code = "1234",
        Host = "xinhua",
        Name = "新华中学"
      };
    }
  }
}