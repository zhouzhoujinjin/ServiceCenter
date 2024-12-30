using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PureCode.DataTransfer.Managers;
using System.Reflection;

namespace PureCode.DataTransfer
{
  public static class SheetExtensions
  {
    public static void AddPureCodeSheet(
        this IServiceCollection services,
        string connectionString,
        IMvcBuilder mvcBuilder)
    {
      var sheetAssembly = Assembly.Load("PureCode.DataTransfer");

      services.AddDbContext<SheetContext>(opt => opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o => o.MigrationsAssembly("PureCode.DataTransfer")));
      services.AddScoped<Exchanger>();
      services.AddScoped<SheetManager>();
      mvcBuilder.AddApplicationPart(sheetAssembly);
    }
  }
}