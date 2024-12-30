using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace PureCode.Utils.Captcha
{
  public static class HeiCaptchaExtension
  {
    /// <summary>
    /// 启用HeiCaptcha
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddHeiCaptcha(this IServiceCollection services)
    {
      if (services == null)
      {
        throw new ArgumentNullException(nameof(services));
      }

      services.AddScoped<SecurityCodeHelper>();
      return services;
    }
  }


}
