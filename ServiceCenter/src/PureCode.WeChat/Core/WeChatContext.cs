using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PureCode.WeChat
{
  public class WeChatContext : AccountInfo
  {
    public string AccessToken { get; set; }
    public DateTime AccessTokenExpiredTime { get; set; }
  }

  public class WeChatUser
  {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public string SessionKey { get; set; }
    public string OpenId { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public string? UnionId { get; set; }
  }

  public class WeChatContainer
  {
    private readonly Dictionary<string, WeChatContext> contexts;
    private readonly IWeChatUserStore userStore;
    private readonly IContextStore contextStore;

    public WeChatContainer(IWeChatUserStore userStore, IContextStore contextStore)
    {
      contexts = new Dictionary<string, WeChatContext>();
      this.userStore = userStore;
      this.contextStore = contextStore;
    }

    public void Clear()
    {
      contexts.Clear();
    }

    public WeChatContext GetContext(string appId)
    {
      if (appId == null)
      {
        throw new ArgumentNullException(nameof(appId), $"{nameof(appId)} is null");
      }
      var exists = contexts.TryGetValue(appId, out var context);
      if (!exists)
      {
        throw new NullReferenceException($"{appId} {nameof(WeChatContext)} is not found");
      }
      return context;
    }

    public IEnumerable<WeChatContext> GetContexts(string accountType)
    {
      return contexts.Values.Where(x => x.AccountType == Enum.Parse<AccountType>(accountType));
    }

    public WeChatContext GetContextByName(string name)
    {
      if (name == null)
      {
        throw new ArgumentNullException(nameof(name), $"{nameof(name)} is null");
      }
      var context = contexts.Values.Where(x => name.ToLower() == x.AppName.ToLower()).FirstOrDefault();

      return context;
    }

    public async Task UpdateContextAccessToken(string appId, string accessToken, int expiresIn)
    {
      // 确保过期时间小于真实过期时间
      var expiredTime = DateTime.Now.AddSeconds(expiresIn - 10);
      var context = GetContext(appId);
      if (context != null)
      {
        context.AccessToken = accessToken;
        context.AccessTokenExpiredTime = expiredTime;
      }
      await contextStore?.SaveContextAsync(appId, accessToken, expiredTime);
    }

    public async Task<WeChatUser> GetUserAsync(string appId, string openId)
    {
      return await userStore.GetUserAsync(appId, openId);
    }

    public async Task StoreUserAsync(string appId, WeChatUser user)
    {
      await userStore.SaveUserAsync(appId, user);
    }

    public void Register(AccountInfo accountInfo)
    {
      var context = new WeChatContext
      {
        Latest = accountInfo.Latest,
        AppId = accountInfo.AppId,
        AgentId = accountInfo.AgentId,
        Secret = accountInfo.Secret,
        AppType = accountInfo.AppType,
        AccountType = accountInfo.AccountType,
        AppName = accountInfo.AppName,
        TemplateMessageInfos = accountInfo.TemplateMessageInfos
      };
      var key = accountInfo.AppId;
      if (context.AgentId != default)
      {
        key = $"{key}.{context.AgentId}";
      }
      var storedContext = contextStore?.GetContext(key);
      if (storedContext != null && storedContext.AccessToken != null)
      {
        context.AccessToken = storedContext.AccessToken;
        context.AccessTokenExpiredTime = storedContext.AccessTokenExpiredTime;
      }
      contexts[key] = context;
    }
  }
}