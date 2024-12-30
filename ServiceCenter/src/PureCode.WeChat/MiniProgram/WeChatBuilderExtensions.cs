using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PureCode.Utils;
using System.Linq;

namespace PureCode.WeChat.MiniProgram
{
  public static class WeChatBuilderExtensions
  {
    public static WeChatBuilder WithMiniProgram(this WeChatBuilder builder)
    {
      builder.Services.AddScoped<WeChatMiniProgramApi>();
      return builder;
    }

    public static WeChatApplication UseMiniProgram(this IApplicationBuilder app)
    {
      var wechatContainer = app.ApplicationServices.GetService<WeChatContainer>();
      var optionsAccessor = app.ApplicationServices.GetService<IOptionsMonitor<WeChatOptions>>();
      optionsAccessor!.OnChange((options) =>
      {
        wechatContainer!.Clear();
        options.Items.Where(x => x.AccountType == AccountType.MiniProgram).ForEach(o =>
         {
           wechatContainer.Register(o);
         });
      });
      optionsAccessor!.CurrentValue.Items.Where(x => x.AccountType == AccountType.MiniProgram).ForEach(o =>
      {
        wechatContainer!.Register(o);
      });

      return new WeChatApplication
      {
        WeChatContainer = wechatContainer!,
        Options = optionsAccessor.CurrentValue
      };
    }

    public static WeChatApplication WithMiniProgram(this WeChatApplication app)
    {
      app.Options.Items.Where(x => x.AccountType == AccountType.MiniProgram).ForEach(o =>
      {
        app.WeChatContainer.Register(o);
      });

      return app;
    }
  }
}