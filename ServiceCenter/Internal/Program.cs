using Internal;
using Microsoft.OpenApi.Models;
using PureCode.Core;
using PureCode.Core.DepartmentFeature;
using PureCode.Core.MenuFeature;
using PureCode.Core.SettingFeature;
using PureCode.Core.TreeFeature;
using PureCode.Core.UserFeature;
using PureCode.Core.ValueSpaceFeature;


var builder = WebApplication.CreateBuilder(args);

#region 配置服务器

builder.WebHost.ConfigureKestrel(options =>
{
  // 取消请求大小限制
  options.Limits.MaxRequestBodySize = null;
  // 增加请求缓存大小
  options.Limits.MaxRequestBufferSize = 1024 * 1024 * 4;
}).UseUrls(builder.Configuration.GetValue<string>("Urls") ?? "http://127.0.0.1:7827");

#endregion 配置服务器

#region 通过环境变量设置选项

// 环境变量中的 "." 需要使用双下划线 "__" 替代。
builder.Configuration.AddEnvironmentVariables();

#endregion 通过环境变量设置选项

#region 日志设置

// Logging 输出到命令行，或者添加其他输出位置
builder.Logging.AddConsole();
// 如文件:
// 需要配合 Serilog.Extensions.Logging.File，appsettings.json 需添加以下配置内容：
// "Logging": {
//   "PathFormat": "logs/log-{Date}.txt",
//   "OutputTemplate": "{Timestamp:o} {RequestId,13} [{Level:u3}] {Message} ({EventId:x8}) {Properties:j}{NewLine}{Exception}",
//   "IncludeScopes": true,
//   "LogLevel": {
//     "Default": "Warning"
//   }
// }

// builder.Logging.AddFile(builder.Configuration.GetSection("Logging"));

#endregion 日志设置

builder.Services.AddControllers();

// 此处添加 PureCode 库及其 Features
builder.Services.AddPureCode(builder.Configuration).WithSetting().WithTree().WithValueSpace().WithUser().WithNavMenu().WithDepartment();//.WithAI()

#region 如果需要，增加 OpenId 认证设置

//if (builder.Configuration.GetSection("OAuthOptions").Exists())
//{
//  authenticationBuilder.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
//  {
//    options.MetadataAddress = builder.Configuration.GetValue<string>("OAuthOptions:BaseUrl") + "/.well-known/openid-configuration";
//    options.ClientId = builder.Configuration.GetValue<string>("OAuthOptions:ClientId");
//    options.ClientSecret = builder.Configuration.GetValue<string>("OAuthOptions:ClientSecret");
//    options.ResponseType = "code";
//    options.CallbackPath = builder.Configuration.GetValue<string>("OAuthOptions:CallbackPath");
//    var scope = builder.Configuration.GetValue<List<string>?>("OAuthOptions:Scope");
//    if (scope != null)
//    {
//      scope.ForEach(s => options.Scope.Add(s));
//    }
//  });
//}

#endregion 如果需要，增加 OpenId 认证设置

#region 如果需要，增加 Cors 设置

//var allowOrigins = builder.Configuration.GetValue<string>("AllowOrigins")?.Split(",");
//if (allowOrigins != null && allowOrigins.Any())
//{
//  builder.Services.AddCors(options =>
//  {
//    options.AddPolicy(ClaimNames.DefaultCorsPolicy, builder => builder.WithOrigins(allowOrigins));
//  });
//}

#endregion 如果需要，增加 Cors 设置

#region 增加本项目自定义的服务注册

// 按实际需要修改

builder.Services.AddInternal(); 

builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" }); });

#endregion 增加本项目自定义的服务注册

var app = builder.Build();

#region 添加中间件，顺序不要变

app.UseDefaultFiles();
app.UseStaticFiles();
app.UsePureCode().UseUser().UseValueSpace();
app.UseRouting();

#region 如果需要，增加 Cors 设置

//if (allowOrigins != null)
//{
//  app.UseCors(ClaimNames.DefaultCorsPolicy);
//}

#endregion 如果需要，增加 Cors 设置

app.UseAuthentication();
app.UseAuthorization();

#endregion 添加中间件，顺序不要变

app.MapControllers();
app.MapSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("v1/swagger.json", "My API V1"); });

#region SPA 首页设置

//如果需要，可以根据路径定位到不同的静态主页

//app.MapFallbackToFile("/site1/{*path:nonfile}", "/site1/index.html");
//app.MapFallbackToFile("/site2/{*path:nonfile}", "/site2/index.html");

app.MapFallbackToFile("/index.html");

#endregion SPA 首页设置

app.Run();