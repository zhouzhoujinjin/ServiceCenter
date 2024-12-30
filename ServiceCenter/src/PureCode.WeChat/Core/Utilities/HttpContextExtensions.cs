using Microsoft.AspNetCore.Http;
using System.Linq;

namespace PureCode.WeChat.Utilities
{
  public static class HttpContextExtensions
  {
    public static WeChatContext? GetWeChatContext(this HttpContext context)
    {
      if (context.Items.TryGetValue("WeChatContext", out var weChatContext))
      {
        return (WeChatContext)weChatContext;
      }
      return null;
    }

    public static string? GetMiniProgramVersion(this HttpContext context)
    {
      if (context.Items.TryGetValue("MiniProgramVersion", out var version))
      {
        return (string)version;
      }
      return null;
    }

    public static string? GetOpenId(this HttpContext context)
    {
      var wechatContext = context.GetWeChatContext();
      if (wechatContext != null)
      {
        return context.User?.Claims.FirstOrDefault(c => c.Type == $"wx.{wechatContext.AppId}.openid")?.Value;
      }
      return null;
    }
  }
}