using Approval.Managers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Approval
{
  public static class ServiceExtensions
  {
    public static void AddApprovalServices(this IServiceCollection services, IConfiguration configuration)
    {
      var connectionString = configuration.GetConnectionString("Default");
      services.AddDbContext<ApprovalDbContext>(options =>
          options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o => o.MigrationsAssembly("Letian")));

      services.AddScoped<ApprovalHooksManager>();
      services.AddScoped<ApprovalFlowManager>();
      services.AddScoped<ApprovalManager>();
      services.AddScoped<FileManager>();
      services.AddScoped<TemplateManager>();
    }
  }
}
