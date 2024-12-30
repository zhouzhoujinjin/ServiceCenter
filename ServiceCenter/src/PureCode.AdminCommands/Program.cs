using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PureCode.Core;
using PureCode.Core.Managers;

namespace PureCode.AdminCommands
{
  [Verb("resetAdminPassword")]
  public class ResetAdminPassword
  {
    [Option('p', "password", Required = true, HelpText = "Reset Admin Password as you assigned")]
    public string Password { get; set; }
  }

  [Verb("createAdmin")]
  public class CreateAdmin
  {
    [Option('n', "name", Required = true, HelpText = "The super admin user name")]
    public string Name { get; set; }

    [Option('p', "password", Required = true, HelpText = "The super admin user password")]
    public string Password { get; set; }
  }

  public class Options
  {
    [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
    public bool Verbose { get; set; }
  }

  internal class Program
  {
    private static int Main(string[] args)
    {
      var result = Parser.Default.ParseArguments(args, typeof(ResetAdminPassword), typeof(CreateAdmin));

      var exitCode = result.MapResult(
        (CreateAdmin opts) => ExecuteCreateAdmin(opts, args),
        (ResetAdminPassword opts) => ExecuteResetAdminPassword(opts),
        errors => HandleErrors(errors)
      );
      return exitCode;
    }

    private static void ConfigureServices(IServiceCollection services)
    {
      // configure logging
      services.AddLogging(builder =>
      {
        builder.AddConsole();
        builder.AddDebug();
      });

      // build config
      var configuration = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: false)
          .AddEnvironmentVariables()
          .Build();

      services.AddPureCode(configuration);
    }

    public static int ExecuteCreateAdmin(CreateAdmin opts, string[] args)
    {
      var services = new ServiceCollection();
      ConfigureServices(services);

      // create service provider
      var serviceProvider = services.BuildServiceProvider();

      var userManager = serviceProvider.GetService<UserManager>()!;
      var user = userManager.AddUserAsync(opts.Name, true, opts.Password).GetAwaiter();

      return user.GetResult() != null && user.GetResult()!.Id > 0 ? 0 : -1;
    }

    public static int ExecuteResetAdminPassword(ResetAdminPassword opts)
    {
      return 0;
    }

    public static int HandleErrors(IEnumerable<Error> errors)
    {
      return 0;
    }
  }
}