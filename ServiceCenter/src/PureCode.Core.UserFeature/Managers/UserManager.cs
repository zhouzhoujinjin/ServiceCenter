using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

public class UserQueryModel
{
  public IQueryable<UserEntity> Users { get; set; }
  public IEnumerable<string> ProfileKeyNames { get; set; }
}

public partial class UserManager : UserManager<UserEntity>
{
  private readonly IDistributedCache cache;
  private readonly PureCodeDbContext context;
  private readonly DbSet<UserEntity> userSet;
  private readonly DbSet<UserClaimEntity> userClaimSet;
  private readonly ProfileManager profileManager;
  private readonly ProfileKeyMap profileKeyMap;
  private readonly IEnumerable<IUserQueryFilter> userQueryFilters;

  public UserManager(
    PureCodeDbContext context,
    IUserStore<UserEntity> store,
    IOptions<IdentityOptions> optionsAccessor,
    IPasswordHasher<UserEntity> passwordHasher,
    IEnumerable<IUserValidator<UserEntity>> userValidators,
    IEnumerable<IPasswordValidator<UserEntity>> passwordValidators,
    ILookupNormalizer keyNormalizer,
    IdentityErrorDescriber errors,
    ProfileManager profileManager,
    ProfileKeyMap profileKeyMap,
    IServiceProvider services,
    ILogger<UserManager<UserEntity>> logger,
    IEnumerable<IUserQueryFilter> userQueryFilters,
    IDistributedCache cache
  ) : base(
    store, optionsAccessor, passwordHasher, userValidators,
    passwordValidators, keyNormalizer, errors, services, logger
  )
  {
    this.cache = cache;
    this.context = context;
    this.profileManager = profileManager;
    this.profileKeyMap = profileKeyMap;
    userSet = context.Set<UserEntity>();
    userClaimSet = context.Set<UserClaimEntity>();
    this.userQueryFilters = GetFinalUserQueryFilters(userQueryFilters);
  }

  private IEnumerable<IUserQueryFilter> GetFinalUserQueryFilters(IEnumerable<IUserQueryFilter> originalFilters)
  {
    var ignoreFilters = originalFilters.SelectMany(x => x.IgnoreFilters);
    return originalFilters.Where(x => !ignoreFilters.Any(i => i == x.GetType()));
  }

  public async Task AddOrUpdateClaimAsync(UserEntity user, string type, string value)
  {
    var claim = await userClaimSet.FirstOrDefaultAsync(x => x.UserId == user.Id && x.ClaimType == type);
    if (claim == null)
    {
      await AddClaimAsync(user, new Claim(type, value));
    }
    else
    {
      claim.ClaimValue = value;
      await context.SaveChangesAsync();
    }
  }

  public async Task AddOrUpdateClaimsAsync(UserEntity user, Dictionary<string, string> claims)
  {
    userClaimSet.RemoveRange(userClaimSet.Where(x =>
      x.UserId == user.Id && claims.ContainsKey(x.ClaimType)));
    await context.SaveChangesAsync();
    await AddClaimsAsync(user, claims.ToArray().Select(x => new Claim(x.Key, x.Value)));
  }

  public async Task<IList<Claim>> GetClaimsAsync(ulong userId)
  {
    var entity = await FindByIdAsync(userId.ToString());
    return await GetClaimsAsync(entity);
  }

  public override string? GetUserName(ClaimsPrincipal principal)
  {
    return principal.Claims.Where(x => x.Type == JwtRegisteredClaimNames.UniqueName).Select(x => x.Value)
      .FirstOrDefault();
  }

  public override async Task<UserEntity?> GetUserAsync(ClaimsPrincipal principal)
  {
    if (principal == null)
    {
      throw new ArgumentNullException(nameof(principal));
    }

    var name = GetUserName(principal);
    if (name == null)
    {
      return await Task.FromResult<UserEntity>(null!);
    }

    return await FindByNameAsync(name);
  }

  public async Task<UserModel> GetBriefUserAsync(ulong userId, string? userName = null,
    IEnumerable<string>? profileKeys = null)
  {
    string[] pks = profileKeys == null ? new string[] { SystemProfileKeyCategory.Public } : profileKeys.ToArray();
    var profiles = await profileManager.GetProfilesAsync(userId, pks);
    var user = new UserModel
    {
      Id = userId,
      UserName = userName,
      Profiles = profiles
    };
    return user;
  }

  public async Task<IEnumerable<UserModel>> GetBriefUsersAsync(
    IEnumerable<ulong> userIds,
    IEnumerable<string>? profileNamesOrCategories = null
  )
  {
    var cachedUsers = new List<UserModel>();
    var unCachedUserIds = new List<ulong>();

    var filteredProfileKeys = profileKeyMap.GetFilteredProfileKeyNames(profileNamesOrCategories);
    foreach (var userId in userIds)
    {
      var u = await cache.GetAsync<UserModel>(string.Format(CacheKeys.Users, userId));
      if (u == null)
      {
        unCachedUserIds.Add(userId);
      }
      else
      {
        cachedUsers.Add(new UserModel
        {
          Id = u.Id,
          UserName = u.UserName,
          Profiles = u.Profiles.Where(x => filteredProfileKeys.Contains(x.Key))
            .ToDictionary(x => x.Key, x => x.Value)
        });
      }
    }

    if (unCachedUserIds.Count > 0)
    {
      var userEntities = await Users
        .Where(u => unCachedUserIds.Contains(u.Id) && u.IsDeleted == false)
        .Select(
          u => new UserEntity
          {
            Id = u.Id,
            UserName = u.UserName
          }).ToArrayAsync();

      var uncachedUsers = await GetBriefUsersAsync(userEntities, profileNamesOrCategories, true);
      cachedUsers.AddRange(uncachedUsers);
    }

    return cachedUsers.OrderBy(x => x.UserName);
  }

  public async Task<IEnumerable<UserModel>> GetBriefUsersAsync(
      IEnumerable<UserEntity> userEntities,
      IEnumerable<string>? profileNamesOrCategories = null,
      bool force = false
    )
  {
    var cachedUsers = new List<UserModel>();
    var uncachedUserEntities = userEntities.ToList();
    var filteredProfileKeys = profileKeyMap.GetFilteredProfileKeyNames(profileNamesOrCategories);
    if (!force)
    {
      uncachedUserEntities = new List<UserEntity>();
      foreach (var user in userEntities)
      {
        var u = await cache.GetAsync<UserModel>(string.Format(CacheKeys.Users, user.Id));
        if (u == null)
        {
          uncachedUserEntities.Add(user);
        }
        else
        {
          cachedUsers.Add(new UserModel
          {
            Id = u.Id,
            UserName = u.UserName,
            Profiles = u.Profiles.Where(x => filteredProfileKeys.Contains(x.Key))
              .ToDictionary(x => x.Key, x => x.Value)
          });
        }
      }
    }

    foreach (var userEntity in uncachedUserEntities)
    {
      var user = new UserModel(userEntity.Id, userEntity.UserName);
      var profiles = await profileManager.GetUserProfilesAsync(userEntity.Id);
      user.Profiles = profiles;
      await cache.SetAsync(string.Format(CacheKeys.Users, userEntity.Id), user,
        new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.MaxValue });
      cachedUsers.Add(new UserModel
      {
        Id = user.Id,
        UserName = user.UserName,
        Profiles = user.Profiles.Where(x => filteredProfileKeys.Contains(x.Key))
          .ToDictionary(x => x.Key, x => x.Value)
      });
    }

    return cachedUsers;
  }

  public async Task<(IEnumerable<AdminUserModel>, int)> ListUsersWithRolesAsync(Dictionary<string, object?> conditions,
    int page, int size)
  {
    var userQuery = BuildFindUsersQuery(conditions);
    var users = userQuery.Include(u => u.UserRoles).ThenInclude(ur => ur.Role);

    var count = await users.CountAsync();
    var data = await users.OrderBy(u => u.UserName).Skip(Math.Max(page, 0) * size).Take(size).ToArrayAsync();
    var adminUsers = new List<AdminUserModel>();

    foreach (var u in data)
    {
      var user = await GetBriefUserAsync(u.Id, u.UserName!, new[] { SystemProfileKeyCategory.Public });
      var adminUser = AdminUserModel.FromUser(user!);
      adminUser.Roles = u.UserRoles.ToDictionary(ur => ur.Role!.Name!, ur => ur.Role!.Title);
      adminUser.IsDeleted = u.IsDeleted;
      adminUser.IsVisible = u.IsVisible;
      var profiles = adminUser.Profiles;

      if (profiles != null)
      {
        var r = profiles.TryGetValue(ProfileKeys.CreatorId, out var creatorId);
        adminUser.CreatorId = r && creatorId != null ? ulong.Parse(creatorId.ToString()!) : null;
        r = profiles.TryGetValue(ProfileKeys.CreatorName, out var creatorName);
        adminUser.CreatorName = r && creatorName != null ? creatorName.ToString() : null;
        r = profiles.TryGetValue(ProfileKeys.CreatorName, out var fullName);
      }

      adminUsers.Add(adminUser);
    }

    return (adminUsers, count);
  }

  public async Task<IEnumerable<UserEntity>> GetUsersForClaimTypeAsync(string claimType)
  {
    return await userSet.Where(u => u.UserClaims.Select(x => x.ClaimType).Contains(claimType)).ToListAsync();
  }

  public async Task<IEnumerable<UserModel>> FindUsersAsync(
    Dictionary<string, object?> conditions,
    int page = 1,
    int size = 1000
  )
  {
    var userQuery = BuildFindUsersQuery(conditions);
    var users = await userQuery.Skip(Math.Max(page - 1, 0) * size).Take(size)
      .Select(u => new UserModel { Id = u.Id, UserName = u.UserName })
      .ToArrayAsync();

    await users.ForEachAsync(async x =>
      x.Profiles = (await GetBriefUserAsync(x.Id, x.UserName!, new string[] { SystemProfileKeyCategory.Public }))
        .Profiles);
    return users;
  }

  public async Task<int> FindUsersCountAsync(Dictionary<string, object?> conditions)
  {
    var userQuery = BuildFindUsersQuery(conditions);
    return await userQuery.CountAsync();
  }

  public IQueryable<UserEntity> BuildFindUsersQuery(
    Dictionary<string, object?> conditions
  )
  {
    var userQuery = Users;

    foreach (var filter in userQueryFilters)
    {
      userQuery = filter.QueryFilter(userQuery, conditions);
    }

    return userQuery;
  }

  public async Task<bool> IsExistUserAsync(string userName)
  {
    var user = await FindByNameAsync(userName);
    return user != null;
  }

  public async Task<UserEntity?> AddUserAsync(string userName, bool isVisible, string password = "123456")
  {
    var entity = new UserEntity
    {
      UserName = userName,
      IsDeleted = false,
      IsVisible = isVisible
    };
    entity.PasswordHash = PasswordHasher.HashPassword(entity, password);
    var result = await CreateAsync(entity);
    return result.Succeeded ? entity : null;
  }

  public async Task<bool> SetDeletedAsync(string userName, bool isDeleted)
  {
    var entity = await FindByNameAsync(userName);
    if (entity != null)
    {
      entity.IsDeleted = isDeleted;
      var result = await UpdateAsync(entity);
      return result == IdentityResult.Success;
    }

    return false;
  }

  public async Task<bool> ResetPasswordAsync(string userName)
  {
    var entity = await FindByNameAsync(userName);
    if (entity != null)
    {
      entity.PasswordHash = PasswordHasher.HashPassword(entity, "123456");
      var result = await UpdateAsync(entity);
      return result == IdentityResult.Success;
    }

    return false;
  }

  public async Task UpdateUserRoleAsync(string roleName, IEnumerable<UserModel> users)
  {
    var existUsers = await GetUsersInRoleAsync(roleName);
    if (existUsers.Count > 0)
    {
      foreach (var o in existUsers)
      {
        await RemoveFromRoleAsync(o, roleName);
        await cache.RemoveAsync(string.Format(CacheKeys.UserClaims, o.Id));
      }
    }

    await AddUserRoleAsync(roleName, users);
  }

  public async Task AddUserRoleAsync(string roleName, IEnumerable<UserModel> users)
  {
    foreach (var item in users)
    {
      var user = await FindByIdAsync(item.Id.ToString());
      await AddToRoleAsync(user!, roleName);
      await cache.RemoveAsync(string.Format(CacheKeys.UserClaims, user!.Id));
    }
  }

  public async Task DeleteUserRoleAsync(string roleName)
  {
    var users = await GetUsersInRoleAsync(roleName);
    await users.ForEachAsync(async o =>
    {
      await cache.RemoveAsync(string.Format(CacheKeys.UserClaims, o.Id));
      await RemoveFromRoleAsync(o, roleName);
    });
  }

  public bool HasClaim(ClaimsPrincipal principal, string claimType, string claimValue)
  {
    return principal.Claims.Any(x => x.Type == claimType && x.Value == claimValue);
  }
}