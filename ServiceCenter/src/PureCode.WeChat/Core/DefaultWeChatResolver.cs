using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace PureCode.WeChat
{
  public class DefaultWeChatResolver : IWeChatResolver
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public WeChatContainer Container { get; }

    private readonly WeChatOptions _options;

    public WeChatContext? Context
    {
      get
      {
        WeChatContext? context = null;
        if (_httpContextAccessor.HttpContext.Items.ContainsKey("WeChatContext"))
        {
          context = (WeChatContext)_httpContextAccessor.HttpContext.Items["WeChatContext"];
        }
        return context;
      }
    }

    public async Task<WeChatUser> GetUserAsync(string openId)
    {
      _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(_options.AppIdHeaderKey, out var appId);
      return await Container.GetUserAsync(appId, openId);
    }

    public DefaultWeChatResolver(
      IHttpContextAccessor httpContextAccessor,
      IOptionsMonitor<WeChatOptions> optionsAccessor,
      WeChatContainer container)
    {
      _httpContextAccessor = httpContextAccessor;
      Container = container;
      _options = optionsAccessor.CurrentValue;
    }
  }
}