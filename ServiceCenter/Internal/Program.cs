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

#region ���÷�����

builder.WebHost.ConfigureKestrel(options =>
{
  // ȡ�������С����
  options.Limits.MaxRequestBodySize = null;
  // �������󻺴��С
  options.Limits.MaxRequestBufferSize = 1024 * 1024 * 4;
}).UseUrls(builder.Configuration.GetValue<string>("Urls") ?? "http://127.0.0.1:7827");

#endregion ���÷�����

#region ͨ��������������ѡ��

// ���������е� "." ��Ҫʹ��˫�»��� "__" �����
builder.Configuration.AddEnvironmentVariables();

#endregion ͨ��������������ѡ��

#region ��־����

// Logging ����������У���������������λ��
builder.Logging.AddConsole();
// ���ļ�:
// ��Ҫ��� Serilog.Extensions.Logging.File��appsettings.json ����������������ݣ�
// "Logging": {
//   "PathFormat": "logs/log-{Date}.txt",
//   "OutputTemplate": "{Timestamp:o} {RequestId,13} [{Level:u3}] {Message} ({EventId:x8}) {Properties:j}{NewLine}{Exception}",
//   "IncludeScopes": true,
//   "LogLevel": {
//     "Default": "Warning"
//   }
// }

// builder.Logging.AddFile(builder.Configuration.GetSection("Logging"));

#endregion ��־����

builder.Services.AddControllers();

// �˴���� PureCode �⼰�� Features
builder.Services.AddPureCode(builder.Configuration).WithSetting().WithTree().WithValueSpace().WithUser().WithNavMenu().WithDepartment();//.WithAI()

#region �����Ҫ������ OpenId ��֤����

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

#endregion �����Ҫ������ OpenId ��֤����

#region �����Ҫ������ Cors ����

//var allowOrigins = builder.Configuration.GetValue<string>("AllowOrigins")?.Split(",");
//if (allowOrigins != null && allowOrigins.Any())
//{
//  builder.Services.AddCors(options =>
//  {
//    options.AddPolicy(ClaimNames.DefaultCorsPolicy, builder => builder.WithOrigins(allowOrigins));
//  });
//}

#endregion �����Ҫ������ Cors ����

#region ���ӱ���Ŀ�Զ���ķ���ע��

// ��ʵ����Ҫ�޸�

builder.Services.AddInternal(); 

builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" }); });

#endregion ���ӱ���Ŀ�Զ���ķ���ע��

var app = builder.Build();

#region ����м����˳��Ҫ��

app.UseDefaultFiles();
app.UseStaticFiles();
app.UsePureCode().UseUser().UseValueSpace();
app.UseRouting();

#region �����Ҫ������ Cors ����

//if (allowOrigins != null)
//{
//  app.UseCors(ClaimNames.DefaultCorsPolicy);
//}

#endregion �����Ҫ������ Cors ����

app.UseAuthentication();
app.UseAuthorization();

#endregion ����м����˳��Ҫ��

app.MapControllers();
app.MapSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("v1/swagger.json", "My API V1"); });

#region SPA ��ҳ����

//�����Ҫ�����Ը���·����λ����ͬ�ľ�̬��ҳ

//app.MapFallbackToFile("/site1/{*path:nonfile}", "/site1/index.html");
//app.MapFallbackToFile("/site2/{*path:nonfile}", "/site2/index.html");

app.MapFallbackToFile("/index.html");

#endregion SPA ��ҳ����

app.Run();