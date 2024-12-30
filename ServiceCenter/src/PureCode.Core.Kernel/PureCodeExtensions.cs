using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PureCode.Core.Kernel;
using PureCode.Core.Options;
using PureCode.Core.Utils;
using PureCode.Utils;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PureCode.Core
{
  public static class PureCodeExtensions
  {
    public static PureCodeServiceCollectionBuilder AddPureCode(
        this IServiceCollection services,
        IConfiguration configuration)
    {
      services.Configure<UploadOptions>(options =>
      {
        options.FolderMask = configuration.GetValue<string>("UploadOptions:FolderMask") ?? "";
        options.FileNameGenerator = configuration.GetValue<string>("UploadOptions:FileNameGenerator") ?? "uuid";
        options.Path = configuration.GetValue<string>("UploadOptions:Path") ?? "./uploads";
        options.WebRoot = configuration.GetValue<string>("UploadOptions:WebRoot") ?? "/uploads";
        var path = options.Path.StartsWith("/") ? options.Path : Path.Combine(Directory.GetCurrentDirectory(), options.Path);
        options.AbsolutePath = path;
      });

      var mvcBuilder = services.AddControllers();

      mvcBuilder.AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonOptions.JsonSerializerDefaultOptions.DefaultIgnoreCondition;
        options.JsonSerializerOptions.Encoder = JsonOptions.JsonSerializerDefaultOptions.Encoder;
        JsonOptions.JsonSerializerDefaultOptions.Converters.ForEach(x => options.JsonSerializerOptions.Converters.Add(x));
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonOptions.JsonSerializerDefaultOptions.DictionaryKeyPolicy;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonOptions.JsonSerializerDefaultOptions.PropertyNamingPolicy;
      });

      #region 时间格式化设置

      // Linux 下默认格式不确定
      if (CultureInfo.CurrentCulture.Clone() is CultureInfo culture)
      {
        culture.DateTimeFormat.LongDatePattern = "yyyy-MM-dd";
        culture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
        culture.DateTimeFormat.ShortTimePattern = "HH:mm";
        culture.DateTimeFormat.LongTimePattern = "HH:mm:ss";
        culture.DateTimeFormat.ShortTimePattern = "HH:mm";
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.CurrentCulture = culture;
      }

      #endregion 时间格式化设置

      #region 缓存设置

      services.AddMemoryCache();
      services.AddStackExchangeRedisCache(options =>
      {
        options.Configuration = configuration.GetValue<string>("CacheOptions:RedisCache:Server") + ",abortConnect=true";
        var pwd = configuration.GetValue<string>("CacheOptions:RedisCache:Password");
        if (pwd != null)
        {
          options.Configuration += $",password={pwd}";
        }
        options.InstanceName = configuration.GetValue<string>("CacheOptions:RedisCache:Instance");
      });

      #endregion 缓存设置

      var connectionString = configuration.GetConnectionString("Default");
      var dbType = configuration.GetValue<string>("DatabaseType") ?? "mysql";
      switch (dbType.ToLower())
      {
        case "mysql":
          services.AddDbContext<PureCodeDbContext>(options =>
          {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o => o.MigrationsAssembly(nameof(PureCodeServiceCollectionBuilder)));
          });
          break;

        case "sqlserver":
        case "mssql":
          services.AddDbContext<PureCodeDbContext>(options =>
          {
            options.UseSqlServer(connectionString, o => o.MigrationsAssembly(nameof(PureCodeServiceCollectionBuilder)));
          });
          break;

        case "oracle":
          services.AddDbContext<PureCodeDbContext>(options =>
          {
            options.UseOracle(connectionString, o =>
            {
              o.MigrationsAssembly(nameof(PureCodeServiceCollectionBuilder));
              o.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19);
            });
          });
          break;
      }

      return new PureCodeServiceCollectionBuilder(services, configuration, mvcBuilder);
    }

    public static PureCodeApplicationBuilder UsePureCode(this IApplicationBuilder app)
    {
      // 上传文件配置
      var uploadOptions = app.ApplicationServices.GetService<IOptions<UploadOptions>>()!.Value;
      if (!Directory.Exists(uploadOptions.AbsolutePath))
      {
        try
        {
          Directory.CreateDirectory(uploadOptions.AbsolutePath);
        }
        catch
        {
          throw new PureCodeException($"无法创建 [{uploadOptions.AbsolutePath}] 上传文件夹");
        }
      }

      app.UseStaticFiles(new StaticFileOptions
      {
        FileProvider = new PhysicalFileProvider(uploadOptions.AbsolutePath),
        RequestPath = uploadOptions.WebRoot
      });

      // 手动初始化全局控制器的单例数据，避免锁

      using var scope = app.ApplicationServices.CreateScope();
      var builder = new PureCodeApplicationBuilder(app, scope);

      var dbContext = scope.ServiceProvider.GetRequiredService<PureCodeDbContext>();

      dbContext.FindModelCreations();

      return builder;
    }
  }
}