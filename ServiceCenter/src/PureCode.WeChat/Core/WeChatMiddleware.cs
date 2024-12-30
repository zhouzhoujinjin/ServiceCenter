using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PureCode.WeChat
{
  public class WeChatMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly WeChatContainer _container;
    private readonly WeChatOptions _options;

    public WeChatMiddleware(
      RequestDelegate next,
      IOptionsMonitor<WeChatOptions> optionsAccessor,
      WeChatContainer container)
    {
      _next = next;
      _options = optionsAccessor.CurrentValue;
      _container = container;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      var byReferer = false;
      if (context.Request.Headers.TryGetValue("Referer", out var referer))
      {
        var regex = new Regex("^https://servicewechat.com/([^/]+)/(.+)/.+$");
        var match = regex.Match(referer);
        if (match.Success && match.Groups.Count == 3 && match.Groups[1].Value.StartsWith("wx"))
        {
          var weChatContext = _container.GetContext(match.Groups[1].Value);
          context.Items.Add("WeChatContext", weChatContext);
          context.Items.Add("MiniProgramVersion", match.Groups[2].Value);
          byReferer = true;
        }
      }

      if (!byReferer && !string.IsNullOrEmpty(_options.AppIdHeaderKey) && context.Request.Headers.TryGetValue(_options.AppIdHeaderKey, out var appId))
      {
        var weChatContext = _container.GetContext(appId);
        context.Items.Add("MiniProgramVersion", "postman/curl");
        context.Items.Add("WeChatContext", weChatContext);
      }

      await _next(context);
    }
  }
}