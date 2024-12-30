using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PureCode.Core.Entities;
using System.Reflection;

namespace PureCode.Core
{
  public class PureCodeDbContext : IdentityDbContext<
      UserEntity, RoleEntity, ulong, UserClaimEntity, UserRoleEntity,
      IdentityUserLogin<ulong>, RoleClaimEntity, IdentityUserToken<ulong>>
  {
    private readonly string dbType;

    public PureCodeDbContext(DbContextOptions<PureCodeDbContext> options, IConfiguration configuration) : base(options)
    {
      dbType = configuration.GetValue<string>("DatabaseType")?.ToLower() ?? "mysql";
    }

    private static readonly List<MethodInfo> moduleOnModelCreatings = new();

    public void FindModelCreations()
    {
      var assemblies = AppDomain.CurrentDomain.GetAssemblies();
      foreach (var assembly in assemblies)
      {
        var modelCreations = assembly.DefinedTypes.Where(x => x.IsClass && x.IsAssignableTo(typeof(IModelCreation))).ToArray();
        foreach (var creation in modelCreations)
        {
          var method = creation.AsType().GetMethod("OnModelCreating", BindingFlags.Public | BindingFlags.Static);
          if (method != null)
          {
            moduleOnModelCreatings.Add(method);
          }
        }
      }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      moduleOnModelCreatings.ForEach(method =>
      {
        method.Invoke(null, new object[] { builder, dbType });
      });
    }
  }
}