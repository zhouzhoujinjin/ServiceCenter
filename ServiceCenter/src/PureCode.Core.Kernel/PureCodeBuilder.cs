using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureCode.Core
{
  public class PureCodeServiceCollectionBuilder
  {
    public IServiceCollection Services { get; }

    public IMvcBuilder MvcBuilder { get; }
    public IConfiguration Configuration { get; }

    public PureCodeServiceCollectionBuilder(IServiceCollection services, IConfiguration configuration, IMvcBuilder mvcBuilder)
    {
      Services = services;
      Configuration = configuration;
      MvcBuilder = mvcBuilder;
    }
  }

  public class PureCodeApplicationBuilder
  {
    public IApplicationBuilder App { get; }
    public IServiceScope ServiceScope { get; }

    public PureCodeApplicationBuilder(IApplicationBuilder app, IServiceScope serviceScope)
    {
      App = app;
      ServiceScope = serviceScope;
    }
  }
}