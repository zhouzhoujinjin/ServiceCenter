using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PureCode.Core.Entities;
using PureCode.Core.Kernel;
using PureCode.Core.MenuFeature;
using PureCode.Core.Models;
using PureCode.Core.TreeFeature;

using PureCode.Utils;

namespace PureCode.Core.Managers
{
  public class MenuManager(PureCodeDbContext context, TreeNodeManager treeNodeManager, RoleManager roleManager, IDistributedCache cache)
  {
    private readonly DbSet<MenuItemEntity> menuItemSet = context.Set<MenuItemEntity>();
    public const string TreeType = "menuItem";

    public async Task<List<MenuItemModel>?> GetMenuAsync()
    {
      var menuItems = await cache.GetAsync(
        CacheKeys.FullMenu,
        async () =>
        {
          var items = await treeNodeManager.GetHierarchicalAsync<MenuItemEntity>(
            TreeType, null,
            async (instanceIds) => await menuItemSet.AsNoTracking().ToListAsync()
          );
          var menuItems = new List<MenuItemModel>();
          TreeNodeManager.LoopNodeToOtherType(items, (item) =>
        {
          var model = new MenuItemModel
          {
            Id = item.Data!.Id,
            Icon = item.Data!.Icon,
            IconType = item.Data.IconType,
            Uri = item.Data.Uri,
            Title = item.Data.Title,
            Type = item.Data.Type,
            ChildrenInvisible = (bool)(item.ExtendData?.GetValueOrDefault(ExtendDataKeys.ChildrenInvisible, false) ?? false),
            Invisible = (bool)(item.ExtendData?.GetValueOrDefault(ExtendDataKeys.Invisible, false) ?? false),
          };
          return model;
        }, menuItems);
          return menuItems;
        },
       new DistributedCacheEntryOptions
       {
         AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(86400)
       }
     );
      return menuItems;
    }

    public async Task UpdateMenuItemAsync(MenuItemModel item)
    {
      var entity = await menuItemSet.Where(x => x.Id == item.Id).FirstOrDefaultAsync();
      var treeNode = await treeNodeManager.GetTreeNodeAsync(TreeType, entity?.Id);
      if (entity == null || treeNode == null) { return; }
      entity.Title = item.Title;
      entity.Uri = item.Uri;
      entity.Icon = item.Icon ?? "";
      entity.IconType = item.IconType;
      entity.IsBlank = item.IsBlank;
      menuItemSet.Update(entity);
      if (item.ChildrenInvisible || item.Invisible)
      {
        treeNode.ExtendData![ExtendDataKeys.ChildrenInvisible] = item.ChildrenInvisible;
        treeNode.ExtendData![ExtendDataKeys.Invisible] = item.Invisible;
      }
      else
      {
        treeNode.ExtendData = new Dictionary<string, object>();
      }
      await treeNodeManager.UpdateExtendDataAsync(treeNode.Id, treeNode.ExtendData);
      await context.SaveChangesAsync();
      await cache.RemoveAsync(CacheKeys.FullMenu);
    }

    public async Task UpdateHierarchicalAsync(ulong itemId, ulong? parentItemId)
    {
      await treeNodeManager.UpdateHierarchicalAsync(TreeType, itemId, parentItemId);
      await cache.RemoveAsync(CacheKeys.FullMenu);
    }

    public async Task<ICollection<MenuItemModel>?> FilterMenusAsync(ICollection<MenuItemModel>? routes, UserEntity user)
    {
      var roles = await roleManager.GetUserRolesAsync(user.Id);
      var allClaims = new HashSet<string>();
      foreach (var r in roles)
      {
        (await roleManager.GetClaimsAsync(r))
            .Where(c => c.Type == PermissionClaimNames.RoutePermission)
            .Select(c => c.Value)
            .ForEach(c => allClaims.Add(c));
      }

      return FilterRoutes(routes, allClaims);
    }

    private ICollection<MenuItemModel>? FilterRoutes(IEnumerable<MenuItemModel>? routes, IEnumerable<string> includePaths)
    {
      var resultRoutes = new List<MenuItemModel>();

      routes?.ForEach(r =>
        {
          if (includePaths.Contains(r.Uri, StringComparer.OrdinalIgnoreCase))
          {
            resultRoutes.Add(r);
            r.Children = FilterRoutes(r.Children, includePaths);
          }
        });
      return resultRoutes;
    }

    public async Task DeleteMenuAsync(ulong itemId)
    {
      var entity = await menuItemSet.FirstOrDefaultAsync(x => x.Id == itemId);
      if (entity != null)
      {
        menuItemSet.Remove(entity);
        await treeNodeManager.RemoveNodeAsync(TreeType, itemId);
        await context.SaveChangesAsync();
        await cache.RemoveAsync(CacheKeys.FullMenu);
      }
    }

    public async Task AddMenuAsync(MenuItemModel item)
    {
      var entity = new MenuItemEntity
      {
        Title = item.Title,
        IconType = item.IconType,
        Icon = item.Icon,
        Uri = item.Uri,
        Type = item.Type,
        IsBlank = item.IsBlank,
      };
      menuItemSet.Add(entity);
      await context.SaveChangesAsync();
      var extendData = new Dictionary<string, object> {
        { ExtendDataKeys.ChildrenInvisible,item.ChildrenInvisible }
        ,{ExtendDataKeys.Invisible,item.Invisible } };
      await treeNodeManager.CreateNodeAsync(TreeType, entity.Id, null, extendData);
      await cache.RemoveAsync(CacheKeys.FullMenu);
    }

    public async Task<bool> ExistUri(string uri)
    {
      return await menuItemSet.AnyAsync(x => x.Uri == uri);
    }

    public async Task<List<PermissionModel>> GetMenuListAsync()
    {
      var menus = await menuItemSet.Select(m => new PermissionModel
      {
        Name = m.Title,
        Value = m.Uri,
        Group = "菜单项"
      }).ToListAsync();
      return menus;
    }

  }
}