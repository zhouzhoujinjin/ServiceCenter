using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PureCode.Core.Managers;
using PureCode.Core.Models;
using PureCode.Core.TreeFeature;
using PureCode.Utils;

namespace PureCode.Core.DepartmentFeature;

public class DepartmentManager(
  PureCodeDbContext context,
  TreeNodeManager treeNodeManager,
  IDistributedCache cache,
  UserManager userManager)
{
  private readonly DbSet<DepartmentEntity> departmentSet = context.Set<DepartmentEntity>();
  private readonly DbSet<DepartmentUserEntity> departmentUserSet = context.Set<DepartmentUserEntity>();
  public const string TreeType = "department";

  public async Task<ICollection<DepartmentModel>> ListAllAsync()
  {
    var departments = await departmentSet.AsNoTracking().ToListAsync();
    return departments.Select(x => new DepartmentModel
    {
      Id = x.Id,
      Title = x.Title,
      CreatedTime = x.CreatedTime
    }).ToList();
  }

  public async Task<List<UserDepartment>> GetUserDepartments(ulong userId)
  {
    var departments = await departmentUserSet
      .Include(e => e.Department)
      .Where(e => e.UserId == userId)
      .Select(x => new UserDepartment
      {
        DepartmentId = x.DepartmentId,
        Title = x.Department.Title,
        IsUserMajorDepartment = x.IsUserMajorDepartment,
        Level = x.Level,
        Position = x.Position
      })
      .ToListAsync();
    return departments;
  }

  private async Task<IEnumerable<DepartmentUserModel>?> GetDepartmentUsersAsync(ulong departmentId)
  {
    var dict = await departmentUserSet
      .Where(e => e.DepartmentId == departmentId)
      .Select(x => new DepartmentUserModel
      {
        Id= x.Id,
        UserId = x.UserId,
        Level = x.Level,
        Position = x.Position,
        IsUserMajorDepartment = x.IsUserMajorDepartment
      }).ToDictionaryAsync(x => x.UserId, x => x);
    if (!dict.Any())
    {
      return null;
    }

    var users = await userManager.FindUsersAsync(new Dictionary<string, object?>
    {
      { "userIds", string.Join(",", dict.Keys) }
    });

    foreach (var user in users)
    {
      dict[user.Id].UserName = user.UserName;
      dict[user.Id].Profiles = user.Profiles;
    }

    List<DepartmentUserModel> departmentUsers = dict.Values.ToList();
    return departmentUsers;
  }

  public async Task<DepartmentModel> GetDepartmentWithUsersAsync(ulong departmentId)
  {
    var departmentEntities = await cache.GetAsync(
      CacheKeys.DepartmentList,
      async () =>
      {
        var entities = await departmentSet.AsNoTracking().ToListAsync();
        return entities;
      },
      new DistributedCacheEntryOptions
      {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(86400)
      }
    );
    var departmentEntity = departmentEntities!.FirstOrDefault(x => x.Id == departmentId);
    if (departmentEntity == null) return new DepartmentModel();
    var departmentUsers = await GetDepartmentUsersAsync(departmentId);
    return new DepartmentModel
    {
      Id = departmentId,
      Title = departmentEntity.Title,
      Users = departmentUsers
    };
  }

  public async Task<List<DepartmentModel>?> GetDepartmentTreeAsync()
  {
    var departmentTree = await cache.GetAsync(
      CacheKeys.DepartmentTree,
      async () =>
      {
        var items = await treeNodeManager.GetHierarchicalAsync<DepartmentEntity>(
          TreeType, null,
          async (departments) => await departmentSet.AsNoTracking().ToListAsync()
        );
        var departmentItems = new List<DepartmentModel>();
        TreeNodeManager.LoopNodeToOtherType(items, (item) =>
        {
          var model = new DepartmentModel
          {
            Title = item.Data!.Title,
            Id = item.Data.Id
          };
          return model;
        }, departmentItems);
        return departmentItems;
      },
      new DistributedCacheEntryOptions
      {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(86400)
      }
    );
    return departmentTree;
  }

  public async Task UpdateDepartmentAsync(ulong departmentId, DepartmentModel department)
  {
    var entity = await departmentSet.FirstOrDefaultAsync(x => x.Id == departmentId);
    if (entity != null)
    {
      entity.Title = department.Title;
      await context.SaveChangesAsync();
      await cache.RemoveAsync(CacheKeys.DepartmentList);
      await cache.RemoveAsync(CacheKeys.DepartmentTree);
    }
  }

  public async Task UpdateDepartmentUserAsync(ulong departmentId, DepartmentModel department)
  {
    if (department.Users != null)
    {
      var users = await departmentUserSet.Where(x => x.DepartmentId == departmentId).ToListAsync();
      departmentUserSet.RemoveRange(users);
      var departmentUserEntities = new List<DepartmentUserEntity>();
      foreach (var du in department.Users)
      {
        var entity = new DepartmentUserEntity
        {
          DepartmentId = departmentId,
          UserId = du.UserId,
          Position = du.Position,
          Level = du.Level,
          IsUserMajorDepartment = du.IsUserMajorDepartment
        };
        departmentUserEntities.Add(entity);
      }

      await departmentUserSet.AddRangeAsync(departmentUserEntities);
      await context.SaveChangesAsync();
    }
  }

  public async Task AddDepartmentAsync(DepartmentModel model, ulong creatorId)
  {
    var entity = new DepartmentEntity
    {
      Title = model.Title,
      CreatedTime = DateTime.UtcNow,
      CreatorId = creatorId
    };
    departmentSet.Add(entity);
    await context.SaveChangesAsync();
    await treeNodeManager.CreateNodeAsync(TreeType, entity.Id);
    await cache.RemoveAsync(CacheKeys.DepartmentTree);
    await cache.RemoveAsync(CacheKeys.DepartmentList);
  }

  public async Task UpdateHierarchicalAsync(ulong departmentId, ulong? parentDepartmentId)
  {
    await treeNodeManager.UpdateHierarchicalAsync(TreeType, departmentId, parentDepartmentId);
    await cache.RemoveAsync(CacheKeys.DepartmentTree);
  }

  public async Task DeleteDepartmentAsync(ulong departmentId)
  {
    var entity = await departmentSet.FirstOrDefaultAsync(x => x.Id == departmentId);
    if (entity != null)
    {
      departmentSet.Remove(entity);
      await treeNodeManager.RemoveNodeAsync(TreeType, departmentId);
      await context.SaveChangesAsync();
      await cache.RemoveAsync(CacheKeys.DepartmentTree);
      await cache.RemoveAsync(CacheKeys.DepartmentList);
    }
  }

  public async Task<ICollection<UserModel>?> GetAllUsersWithDepartmentsRolesAsync()
  {
    var userModels = await cache.GetAsync(
      CacheKeys.AllUsersWithDepartmentsRoles,
      async () =>
      {
        var dict = new Dictionary<string, object?>
        {
          { "deleted", false },
          { "visible", true }
        };
        var (users, total) = await userManager.ListUsersWithRolesAsync(dict, 0, 10000);
        if (users == null || total == 0) return new List<UserModel>();

        var userList = new List<UserModel>();
        foreach (var user in users!)
        {
          var userDepartments = await GetUserDepartments(user.Id);
          var deptStr = string.Join(",",
            userDepartments.Where(x => x.IsUserMajorDepartment == true).Select(x => x.Title));
          user.Profiles.Add("departmentName", deptStr);
          var userModel = new UserModel
          {
            Id = user.Id,
            UserName = user.UserName,
            Profiles = user.Profiles,
            Roles = user.Roles
          };
          userList.Add(userModel);
        }

        return userList;
      },
      new DistributedCacheEntryOptions
      {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(86400)
      }
    );
    return userModels;
  }
}