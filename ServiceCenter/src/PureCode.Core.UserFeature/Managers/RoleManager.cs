using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using PureCode.Core.Entities;
using PureCode.Core.Kernel;
using PureCode.Core.Models;
using PureCode.Utils;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using PureCode.Core.UserFeature;


namespace PureCode.Core.Managers;

public class RoleManager : RoleManager<RoleEntity>
{
  private readonly IActionDescriptorCollectionProvider provider;
  private readonly UserManager userManager;
  private readonly DbSet<UserRoleEntity> userRoleSet;
  private readonly IDistributedCache cache;

  public RoleManager(
    PureCodeDbContext context,
    IRoleStore<RoleEntity> store,
    IEnumerable<IRoleValidator<RoleEntity>> roleValidators,
    ILookupNormalizer keyNormalizer,
    IdentityErrorDescriber errors,
    IActionDescriptorCollectionProvider provider,
    IDistributedCache cache,
    ILogger<RoleManager<RoleEntity>> logger,
    UserManager userManager
  ) : base(
    store,
    roleValidators,
    keyNormalizer,
    errors,
    logger
  )
  {
    this.provider = provider;
    this.userManager = userManager;
    userRoleSet = context.Set<UserRoleEntity>();
    this.cache = cache;
  }

  public async Task<IEnumerable<string>> GetClaimsAsync(string claimType, IEnumerable<string> roleNames)
  {
    return await Roles.Include(r => r.RoleClaims)
      .Where(r => roleNames.Contains(r.Name))
      .SelectMany(r => r.RoleClaims)
      .Where(c => c.ClaimType == claimType && c.ClaimValue != null)
      .Select(c => c.ClaimValue!).ToArrayAsync();
  }

  public async Task<IEnumerable<RoleEntity>> GetUserRolesAsync(ulong userId)
  {
    return await Roles.Where(x => x.UserRoles.Any(ur => ur.UserId == userId)).ToArrayAsync();
  }

  public async Task<SortedDictionary<string, string>> GetBriefRolesAsync()
  {
    return new SortedDictionary<string, string>(await Roles.ToDictionaryAsync(r => r.Name!, r => r.Title));
  }

  public IEnumerable<PermissionModel> GetPermissionActions()
  {
    var groups = new Dictionary<string, string>();
    var permissions = provider.ActionDescriptors.Items.Where(x => x.EndpointMetadata.Any(y =>
        y is AuthorizeAttribute && (y as AuthorizeAttribute)?.Policy == PermissionClaimNames.ApiPermission))
      .Select(x =>
      {
        var group = (x as ControllerActionDescriptor)?.ControllerName!;
        if (!groups.TryGetValue(group, out var friendlyGroup))
        {
          friendlyGroup =
            ((x as ControllerActionDescriptor)?.ControllerTypeInfo?.GetCustomAttributes(typeof(RouteAttribute), false)
              ?.FirstOrDefault() as RouteAttribute)?.Name ?? group;
          groups[group] = friendlyGroup;
        }

        return new PermissionModel
        {
          Name = x.AttributeRouteInfo!.Name!,
          Value =
            $"{x?.ActionConstraints?.OfType<HttpMethodActionConstraint>().FirstOrDefault()?.HttpMethods.First()} {x!.AttributeRouteInfo.Template}",
          Group = friendlyGroup
        };
      });
    return permissions;
  }

  public async Task<IEnumerable<RoleModel>> GetRolesWithUsersAsync(int page, int size)
  {
    var roles = await Roles.Include(x => x.UserRoles).ThenInclude(x => x.User)
      .Skip(Math.Max(page - 1, 0) * size)
      .Take(size).Select(x => new RoleModel
      {
        Name = x.Name!,
        Title = x.Title,
        Users = x.UserRoles.Where(x => !x.User!.IsDeleted).Select(u => new UserModel
        {
          Id = u.UserId,
          UserName = u.User!.UserName
        }).ToList()
      }).ToListAsync();
    foreach (var role in roles)
    {
      for (var i = 0; i < role.Users!.Count; i++)
      {
        role.Users[i] = await userManager.GetBriefUserAsync(role.Users[i].Id, role.Users[i].UserName,
          new string[] { "fullName", "avatar" });
      }
    }

    return roles;
  }

  public async Task<RoleModel> GetRoleWithUsersAndClaimsAsync(string name, bool withClaims = true)
  {
    var roleEntity = await FindByNameAsync(name);
    var users = await userRoleSet.Include(x => x.User)
      .Where(r => r.RoleId == roleEntity!.Id).Select(x => new UserModel
      {
        Id = x.UserId
      }).ToListAsync();

    var role = new RoleModel
    {
      Name = roleEntity!.Name!,
      Title = roleEntity.Title,
      Users = new List<UserModel>()
    };
    for (var i = 0; i < users.Count; i++)
    {
      role.Users.Add(await userManager.GetBriefUserAsync(users[i].Id, users[i].UserName,
        new string[] { "fullName", "avatar" }));
    }
    if (withClaims)
    {
      var claims = await GetClaimsAsync(roleEntity!);
      role.Claims = claims.Select(c => c.Value).ToList();
    }
    return role;
  }

  public async Task<bool> CheckPermission(UserEntity user, string permission)
  {
    var claims = (await cache.GetAsync(string.Format(CacheKeys.UserClaims, user.Id), async () =>
      {
        var claims = new List<Claim>
        {
          new(JwtRegisteredClaimNames.NameId, user.Id.ToString())
        };
        var userRoleNames = await userManager.GetRolesAsync(user);
        var userRoles = Roles.Where(x => userRoleNames.Contains(x.Name)).ToList();
        foreach (var role in userRoles)
        {
          var roleClaims = await GetClaimsAsync(role);
          var permissions = roleClaims.Where(x => x.Type == PermissionClaimNames.ApiPermission);
          claims.AddRange(permissions);
        }

        return claims.Distinct(EqualityFactory.Create<Claim>((x, y) => x != null && y != null && x.Value == y.Value))
          .Select(x => new KeyValuePair<string, string>(x.Type, x.Value)).ToList();
      }, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3600) }))
      ?.Select(x => new Claim(x.Key, x.Value));
    return claims != null && claims.Any(x => x.Value == permission);
  }

  public IList<Claim> GetClaims(IEnumerable<string> claims)
  {
    IList<Claim> claimList = new List<Claim>();
    claims.ForEach(claim =>
    {
      string[] typeAndValue = claim.Split(new char[] { ',' });
      claimList.Add(new Claim(typeAndValue[0], typeAndValue[1]));
    });
    return claimList;
  }

  public async Task UpdateRoleAsync(string roleName, string title)
  {
    var role = await FindByNameAsync(roleName);
    if (role == null)
    {
      return;
    }

    role.Title = title;
    await UpdateAsync(role);
  }

  public async Task UpdateClaimAsync(string roleName, IList<Claim> claims)
  {
    var role = await FindByNameAsync(roleName);
    var existClaims = await GetClaimsAsync(role);
    await existClaims.ForEachAsync(o => RemoveClaimAsync(role, o));
    await AddClaimsAsync(role, claims);
  }

  public async Task<bool> IsExistRoleAsync(string roleName)
  {
    var user = await Roles.FirstOrDefaultAsync(o => o.Name!.Equals(roleName));
    return user != null;
  }

  public async Task<RoleEntity?> AddRoleAsync(string roleName, string roleTitle)
  {
    var entity = new RoleEntity
    {
      Name = roleName,
      Title = roleTitle
    };
    var ir = await CreateAsync(entity);
    if (ir.Succeeded)
    {
      return entity;
    }

    return null;
  }

  public async Task AddClaimsAsync(RoleEntity role, IList<Claim> claims)
  {
    if (claims.Count > 0)
    {
      foreach (var item in claims)
      {
        await AddClaimAsync(role, item);
      }
    }
  }

  public async Task<bool> DeleteRoleAsync(string roleName)
  {
    var entity = await FindByNameAsync(roleName);
    var ir = await DeleteAsync(entity);
    return ir.Succeeded;
  }

  public async Task DeleteRoleClaimAsync(string roleName)
  {
    var role = await FindByNameAsync(roleName);
    var claims = await GetClaimsAsync(role);
    await claims.ForEachAsync(o => RemoveClaimAsync(role, o));
  }
}