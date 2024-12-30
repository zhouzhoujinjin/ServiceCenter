using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Linq;

namespace PureCode.Utils
{
  public static class HttpContextExtensions
  {
    public static string? GetUserName(this HttpContext ctx)
    {
      return ctx.User?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value;
    }

    public static ulong GetUserId(this HttpContext ctx)
    {
      var id = ctx.User?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.NameId)?.Value;

      var result = ulong.TryParse(id, out var value);

      if (value == 0)
      {
        var srv = ctx.RequestServices.GetService(typeof(ILogger<HttpContext>)) as ILogger<HttpContext>;
        srv?.LogWarning($"[{nameof(GetUserId)}] 不存在登录用户，请检查是否发送登录信息");
      }
      return result ? value : default;
    }
  }
}