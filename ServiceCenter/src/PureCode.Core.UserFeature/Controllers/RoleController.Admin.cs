using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PureCode.Core.Managers;
using PureCode.Core.Models;
using System.Collections.Generic;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace PureCode.Core.Controllers
{
  public partial class RoleController : ControllerBase
  {
    ///// <summary>
    ///// 因循环依赖迁移至MenuController中 
    ///// 获取所有后端的权限点
    ///// </summary>
    ///// <returns></returns>
    //[HttpGet("/api/admin/permissions", Name = "管理员 - 权限点列表")]
    //public AjaxResp<IEnumerable<PermissionModel>> Permissions()
    //{
    //  var allPermissions=new List<PermissionModel>();
    //  var rolePermissions = roleManager.GetPermissionActions();

    //  return new AjaxResp<IEnumerable<PermissionModel>>
    //  {
    //    Data = rolePermissions,
    //  };
    //}

    /// <summary>
    /// 分页获取角色列表（包括角色中的用户）
    /// </summary>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    [HttpGet("/api/admin/roles", Name = "管理员 - 角色列表")]
    public async Task<PagedAjaxResp<RoleModel>> IndexAsync(int page = 1, int size = 1000)
    {
      return new PagedAjaxResp<RoleModel>
      {
        Data = await roleManager.GetRolesWithUsersAsync(page, size),
        Total = await roleManager.Roles.CountAsync(),
        Page = page
      };
    }

    [HttpGet("/api/admin/roles/{name}", Name = "管理员 - 角色详情")]
    public async Task<AjaxResp<RoleModel>> GetRole(string name)
    {
      return new AjaxResp<RoleModel>
      {
        Code = 0,
        Data = await roleManager.GetRoleWithUsersAndClaimsAsync(name)
      };
    }

    [HttpGet("/api/admin/roles/{name}/withoutClaims", Name = "管理员 - 角色详情-无权限")]
    public async Task<AjaxResp<RoleModel>> GetRoleWithoutClaims(string name)
    {
      return new AjaxResp<RoleModel>
      {
        Code = 0,
        Data = await roleManager.GetRoleWithUsersAndClaimsAsync(name, false)
      };
    }

    [HttpPost("/api/admin/roles", Name = "管理员 - 添加角色及权限")]
    public async Task<AjaxResp> Add([FromBody] RoleModel rwuac)
    {
      var role = await roleManager.AddRoleAsync(rwuac.Name, rwuac.Title!);
      if (role != null)
      {
        var claims = roleManager.GetClaims(rwuac.Claims!);
        await roleManager.AddClaimsAsync(role, claims);
        if(rwuac.Users != null)
        {
          await userManager.AddUserRoleAsync(rwuac.Name, rwuac.Users!);
        }
      }
      return new AjaxResp { Message = "保存成功" };
    }

    [HttpPost("/api/admin/roles/{roleName}/users", Name = "管理员 - 分配角色用户")]
    public async Task<AjaxResp> UpdateRoleUsers(string roleName, [FromBody] List<UserModel> users)
    {
      await userManager.DeleteUserRoleAsync(roleName);
      if (users == null || users.Count == 0) return new AjaxResp { Message = "保存成功，已清空角色用户" };
      await userManager.AddUserRoleAsync(roleName, users!);
      return new AjaxResp { Message = "保存成功" };
    }

    [HttpPost("/api/admin/roles/{name}/update", Name = "管理员 - 修改角色")]
    public async Task<AjaxResp> Update(string name, [FromBody] RoleModel roleWithUsersAndClaims)
    {
      var claims = roleManager.GetClaims(roleWithUsersAndClaims.Claims!);
      await roleManager.UpdateRoleAsync(name, roleWithUsersAndClaims.Title!);
      await roleManager.UpdateClaimAsync(name, claims);
      //await userManager.UpdateUserRoleAsync(name, roleWithUsersAndClaims.Users!);
      return new AjaxResp { Message = "保存成功" };
    }

    [HttpGet("/api/admin/roles/{name}/isExist", Name = "管理员 - 判断角色是否存在")]
    public async Task<AjaxResp<bool>> IsExistRole(string name)
    {
      var data = await roleManager.IsExistRoleAsync(name);
      return new AjaxResp<bool> { Data = data };
    }

    [HttpPost("/api/admin/roles/{roleName}/delete", Name = "管理员 - 删除角色")]
    public async Task<AjaxResp> Delete(string roleName)
    {
      await roleManager.DeleteRoleClaimAsync(roleName);
      await userManager.DeleteUserRoleAsync(roleName);
      await roleManager.DeleteRoleAsync(roleName);
      return new AjaxResp { Message = "操作成功" };
    }
  }
}