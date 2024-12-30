using Microsoft.Extensions.Caching.Distributed;
using PureCode.Utils;
using System;
using System.Threading.Tasks;

namespace PureCode.WeChat.Stores
{
  public class DistributedCacheStore : IWeChatUserStore, IContextStore, IJsTicketStore
  {
    private readonly IDistributedCache cache;

    public DistributedCacheStore(IDistributedCache cache)
    {
      this.cache = cache;
    }

    public async Task<WeChatUser> GetUserAsync(string appId, string openId)
    {
      return await cache.GetAsync<WeChatUser>($"{WeChatConsts.OpenIdsCachePrefix}:{appId}:{openId}");
    }

    public async Task<WeChatContext> GetContextAsync(string appId)
    {
      return await cache.GetAsync<WeChatContext>($"{WeChatConsts.ContextsCachePrefix}:{appId}");
    }

    public WeChatContext GetContext(string appId)
    {
      return cache.Get<WeChatContext>($"{WeChatConsts.ContextsCachePrefix}:{appId}");
    }

    public async Task SaveUserAsync(string appId, WeChatUser user)
    {
      await cache.SetAsync($"{WeChatConsts.OpenIdsCachePrefix}:{appId}:{user.OpenId}", user, new DistributedCacheEntryOptions
      {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(7200)
      });
    }

    public async Task SaveContextAsync(string appId, string accessToken, DateTime dateTime)
    {
      await cache.SetAsync($"{WeChatConsts.ContextsCachePrefix}:{appId}", new WeChatContext
      {
        AccessToken = accessToken,
        AppId = appId,
        AccessTokenExpiredTime = dateTime
      }, new DistributedCacheEntryOptions { AbsoluteExpiration = dateTime });
    }

    public async Task SaveTicketAsync(string appId, string ticket, DateTime dateTime)
    {
      await cache.SetAsync($"{WeChatConsts.ContextsCachePrefix}:tickets:{appId}", ticket, new DistributedCacheEntryOptions { AbsoluteExpiration = dateTime });
    }

    public async Task<string> GetTicketAsync(string appId)
    {
      return await cache.GetAsync<string>($"{WeChatConsts.ContextsCachePrefix}:tickets:{appId}");
    }

    public string GetTicket(string appId)
    {
      return cache.Get<string>($"{WeChatConsts.ContextsCachePrefix}:tickets:{appId}");
    }
  }
}