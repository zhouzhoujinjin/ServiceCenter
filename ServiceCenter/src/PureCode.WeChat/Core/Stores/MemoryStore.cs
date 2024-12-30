using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace PureCode.WeChat.Stores
{
  public class MemoryStore : IWeChatUserStore, IContextStore, IJsTicketStore
  {
    private readonly IMemoryCache cache;

    public MemoryStore(IMemoryCache cache)
    {
      this.cache = cache;
    }

    public Task<WeChatContext> GetContextAsync(string appId)
    {
      return Task.FromResult(cache.Get<WeChatContext>($"{WeChatConsts.ContextsCachePrefix}:{appId}"));
    }

    public WeChatContext GetContext(string appId)
    {
      return cache.Get<WeChatContext>($"{WeChatConsts.ContextsCachePrefix}:{appId}");
    }

    public Task<WeChatUser> GetUserAsync(string appId, string openId)
    {
      return Task.FromResult(cache.Get<WeChatUser>($"{WeChatConsts.OpenIdsCachePrefix}:{appId}:{openId}"));
    }

    public Task SaveContextAsync(string appId, string accessToken, DateTime dateTime)
    {
      cache.Set($"{WeChatConsts.ContextsCachePrefix}:{appId}", new WeChatContext
      {
        AccessToken = accessToken,
        AppId = appId,
        AccessTokenExpiredTime = dateTime
      }, dateTime);
      return Task.FromResult(0);
    }

    public Task SaveUserAsync(string appId, WeChatUser user)
    {
      return Task.FromResult(cache.Set($"{WeChatConsts.OpenIdsCachePrefix}:{appId}:{user.OpenId}", user));
    }

    public Task SaveTicketAsync(string appId, string ticket, DateTime dateTime)
    {
      cache.Set($"{WeChatConsts.ContextsCachePrefix}:tickets:{appId}", ticket, dateTime);
      return Task.FromResult(0);
    }

    public Task<string> GetTicketAsync(string appId)
    {
      return Task.FromResult(cache.Get<string>($"{WeChatConsts.ContextsCachePrefix}:tickets:{appId}"));
    }

    public string GetTicket(string appId)
    {
      return cache.Get<string>($"{WeChatConsts.ContextsCachePrefix}:tickets:{appId}");
    }
  }
}