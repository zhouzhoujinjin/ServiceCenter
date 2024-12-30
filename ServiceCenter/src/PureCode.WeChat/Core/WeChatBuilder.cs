using Microsoft.Extensions.DependencyInjection;

namespace PureCode.WeChat
{
  public class WeChatBuilder
  {
    /// <summary>
    /// The services being configured.
    /// </summary>
    public IServiceCollection Services { get; }

    public WeChatBuilder(IServiceCollection services)
    {
      Services = services;
    }
  }
}