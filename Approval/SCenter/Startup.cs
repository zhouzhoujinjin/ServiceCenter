using SCenter;
using CyberStone.Core;
using Hangfire;
using Microsoft.IdentityModel.Logging;
using Approval;

namespace SCenter
{
  public class Startup
  {
    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
      Configuration = configuration;
      _environment = environment;
    }

    public IConfiguration Configuration { get; }
    private bool enableHangfire;

    private IWebHostEnvironment _environment;
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddHttpClient();
      services.AddCyberStone(Configuration);
      services.AddSCenter(Configuration);
      services.AddApprovalServices(Configuration);


      var connection = Configuration.GetConnectionString("Default");
    //  enableHangfire = Configuration.GetValue<bool>("EnableHangfire");
    //  if (enableHangfire)
    //  {
    //    //����Hangfire
    //    var cs = Configuration.GetConnectionString("Hangfire");
    //    //services.AddHostedService<ApprovalBackgroundService>();
    //    services.AddHangfire(x => x.UseStorage(new MySqlStorage(
    //                          cs,
    //                          new MySqlStorageOptions
    //                          {
    //                            TransactionIsolationLevel = IsolationLevel.ReadCommitted, // ������뼶��Ĭ���Ƕ�ȡ���ύ��
    //                          QueuePollInterval = TimeSpan.FromSeconds(15),             //- ��ҵ������ѯ�����Ĭ��ֵΪ15�롣
    //                          JobExpirationCheckInterval = TimeSpan.FromHours(1),       //- ��ҵ���ڼ������������ڼ�¼����Ĭ��ֵΪ1Сʱ��
    //                          CountersAggregateInterval = TimeSpan.FromMinutes(5),      //- �ۺϼ������ļ����Ĭ��Ϊ5���ӡ�
    //                          PrepareSchemaIfNecessary = true,                          //- �������Ϊtrue���򴴽����ݿ��Ĭ����true��
    //                          DashboardJobListLimit = 50000,                            //- �Ǳ����ҵ�б����ơ�Ĭ��ֵΪ50000��
    //                          TransactionTimeout = TimeSpan.FromMinutes(1),             //- ���׳�ʱ��Ĭ��Ϊ1���ӡ�
    //                          TablesPrefix = "Hangfire"
    //                          }
    //                          )));
    //    services.AddHangfireServer();
    //  }
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
    {
      if (env.IsDevelopment())
      {
        IdentityModelEventSource.ShowPII = true;
        app.UseDeveloperExceptionPage();
      }

      app.UseDefaultFiles();
      app.UseStaticFiles();
      
      app.UsePureCode();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        endpoints.MapFallbackToFile("/index.html");
        if (enableHangfire)
        {
          endpoints.MapHangfireDashboard();
        }
      });

    }

  }
}