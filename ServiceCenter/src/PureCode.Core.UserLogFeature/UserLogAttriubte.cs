using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using PureCode.Core.Managers;
using PureCode.Core.Models;
using PureCode.Utils;

namespace PureCode.Core.Utils
{
  public class UserLogAttribute : ActionFilterAttribute
  {
    protected DateTime startTime;

    private bool needLog;

    public UserLogAttribute()
    {
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      if (filterContext.ActionDescriptor is Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)
      {
        var descriptor = (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)filterContext.ActionDescriptor;
        needLog = descriptor.MethodInfo.CustomAttributes.Any(x => x.AttributeType == typeof(UserLogAttribute));
      };
      startTime = DateTime.Now;
    }

    public override async Task OnResultExecutionAsync(ResultExecutingContext filterContext, ResultExecutionDelegate next)
    {
      var httpContext = filterContext.HttpContext;

      if (!needLog)
      {
        await next();
        return;
      }

      // 防止用户反复刷新页面，导致存入数据库的日志过多
      var exists = httpContext.Request.Headers.TryGetValue("UrlReferrer", out var referrer);
      if (exists && httpContext.Request.GetEncodedUrl().Equals(referrer))
      {
        await next();
        return;
      }

      await next();

      var svc = filterContext.HttpContext.RequestServices;
      var manager = (svc.GetService(typeof(UserLogManager)) as UserLogManager)!;
      var duration = (DateTime.Now - startTime);
      var createdAt = DateTime.Now;

      var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
      var device = userAgent.Contains("micromessenger") ? "wechat" : "browser";
      device += userAgent.Contains("iphone") ? "/ios" : userAgent.Contains("android") ? "/android" : "/pc";
      var userId = httpContext.GetUserId();

      var userLog = new UserLogModel()
      {
        Url = httpContext.Request.GetDisplayUrl(),
        Method = httpContext.Request.Method,
        Device = device,
        Data = httpContext.Request.QueryString.ToString(),
        Ip = httpContext.Connection.RemoteIpAddress?.ToString(),
        UserAgent = userAgent,
        Duration = (int)duration.TotalMilliseconds,
        UserId = userId,
        CreatedTime = createdAt
      };
      await manager.AddLog(userLog);
    }
  }
}