using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PureCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PureCode.WeChat.Work
{
  public static class WeChatBuilderExtensions
  {
    public static WeChatBuilder WithWork(this WeChatBuilder builder)
    {
      builder.Services.AddScoped<WeChatWorkApi>();
      return builder;
    }

    public static WeChatApplication UseWork(this IApplicationBuilder app, Action<WeChatWorkApi, IEnumerable<string>>? action = null)
    {
      var wechatContainer = app.ApplicationServices.GetService<WeChatContainer>();
      var optionsAccessor = app.ApplicationServices.GetService<IOptions<WeChatOptions>>();
      var agents = optionsAccessor!.Value.Items.Where(x => x.AccountType == AccountType.Work);
      agents.ForEach(o =>
      {
        wechatContainer!.Register(o);
      });
      var workApi = app.ApplicationServices.GetService<WeChatWorkApi>();
      if (action != null)
      {
        action.Invoke(workApi!, agents.Select(x => x.AppName));
      }

      return new WeChatApplication
      {
        WeChatContainer = wechatContainer!,
        Options = optionsAccessor.Value
      };
    }

    public static WeChatApplication WithWork(this WeChatApplication app)
    {
      app.Options.Items.Where(x => x.AccountType == AccountType.Work).ForEach(o =>
      {
        app.WeChatContainer.Register(o);
      });

      return app;
    }
  }
}