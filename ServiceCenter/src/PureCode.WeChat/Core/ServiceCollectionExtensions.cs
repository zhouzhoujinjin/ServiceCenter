using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using PureCode.WeChat;
using PureCode.WeChat.Stores;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
  public static class ServiceCollectionExtensions
  {
    public static WeChatBuilder AddWeChat(
        this IServiceCollection services,
        IConfigurationSection config
    )
    {
      return services.AddWeChat(config, null);
    }

    public static WeChatBuilder AddWeChat(
        this IServiceCollection services,
        IConfigurationSection config,
        Action<WeChatOptions> setupAction)
    {
      services.AddHttpClient(nameof(WeChatClient));
      services.Configure<WeChatOptions>(config);
      services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
      services.AddSingleton<WeChatContainer>();
      services.AddMemoryCache();
      services.AddSingleton<IWeChatUserStore, MemoryStore>();
      services.AddSingleton<IContextStore, MemoryStore>();
      services.AddSingleton<IJsTicketStore, MemoryStore>();
      services.AddSingleton<IWeChatClient, WeChatClient>();
      services.AddSingleton<IWeChatResolver, DefaultWeChatResolver>();

      services.AddSingleton<IWeChatNotifyClient, WeChatNotifyClient>();

      if (setupAction != null)
      {
        services.Configure(setupAction);
      }

      var builder = new WeChatBuilder(services);

      return builder;
    }

    public static WeChatBuilder WithRedis(this WeChatBuilder builder)
    {
      builder.Services.AddSingleton<IWeChatUserStore, DistributedCacheStore>();
      builder.Services.AddSingleton<IContextStore, DistributedCacheStore>();
      builder.Services.AddSingleton<IJsTicketStore, DistributedCacheStore>();
      return builder;
    }

    public static WeChatApplication UseWeChat(this IApplicationBuilder app)
    {
      app.UseMiddleware<WeChatMiddleware>();
      var wechatContainer = app.ApplicationServices.GetService<WeChatContainer>();
      var optionsAccessor = app.ApplicationServices.GetService<IOptions<WeChatOptions>>();
      return new WeChatApplication
      {
        WeChatContainer = wechatContainer,
        Options = optionsAccessor.Value
      };
    }
  }
}