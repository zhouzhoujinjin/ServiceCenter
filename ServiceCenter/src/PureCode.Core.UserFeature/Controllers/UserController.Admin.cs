using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PureCode.Core.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PureCode.Core.Controllers;

public partial class UserController
{
  [HttpGet("/api/admin/users", Name = "管理员 - 用户列表")]
  public async Task<PagedAjaxResp<AdminUserModel>> AdminListUsersAsync(string? query = null, int page = 0,
    int size = 10)
  {
    var dict = new Dictionary<string, object?>
    {
      { "deleted", false },
      { "visible", true }
    };
    if (query != null)
    {
      dict["any"] = query;
    }

    var (users, total) = await userManager.ListUsersWithRolesAsync(dict, page, size);

    return new PagedAjaxResp<AdminUserModel>
    {
      Total = total,
      Page = page,
      Data = users
    };
  }

  [HttpGet("/api/admin/users/deleted", Name = "管理员 - 已删除用户列表")]
  public async Task<PagedAjaxResp<AdminUserModel>> AdminListDeletedUsersAsync(string? query = null, int page = 0,
    int size = 20)
  {
    var dict = new Dictionary<string, object?>
    {
      { "deleted", true }
    };
    if (query != null)
    {
      dict["fullName"] = query;
      dict["userName"] = query;
    }

    var (users, total) = await userManager.ListUsersWithRolesAsync(dict, page, size);

    return new PagedAjaxResp<AdminUserModel>
    {
      Total = total,
      Page = page,
      Data = users
    };
  }
                                                
  [HttpGet("/api/admin/users/invisible", Name = "管理员 - 已禁用用户列表")]
  public async Task<PagedAjaxResp<AdminUserModel>> AdminListInvisibleUsersAsync(string? query = null, int page = 0,
    int size = 20)
  {
    var dict = new Dictionary<string, object?>
    {
      { "deleted", false },
      { "visible", false }
    };
    if (query != null)
    {
      dict["fullName"] = query;
      dict["userName"] = query;
    }

    var (users, total) = await userManager.ListUsersWithRolesAsync(dict, page, size);

    return new PagedAjaxResp<AdminUserModel>
    {
      Total = total,
      Page = page,
      Data = users
    };
  }

  [HttpPost("/api/admin/users", Name = "管理员 - 添加用户")]
  public async Task<AjaxResp<UserModel>> AdminCreateUserAsync([FromBody] UserModel user)
  {
    if (string.IsNullOrEmpty(user.UserName))
    {
      return new AjaxResp<UserModel>
      {
        Code = 403,
        Message = "用户名为空，无法添加"
      };
    }
    var data = await userManager.IsExistUserAsync(user.UserName);
    if (data)
    {
      return new AjaxResp<UserModel>
      {
        Code = 403,
        Message = "用户名已存在，无法添加"
      };
    }

    var entity = await userManager.AddUserAsync(user.UserName, true);
    var userId = entity?.Id ?? 0;

    if (userId > 0)
    {
      await profileManager.AddProfilesAsync(userId, user.Profiles);
    }

    user.Id = userId;
    return new AjaxResp<UserModel>
    {
      Message = "保存成功",
      Data = user
    };
  }

  [HttpPost("/api/admin/users/{userName}", Name = "管理员 - 修改用户")]
  public async Task<AjaxResp<UserModel>> AdminUpdateAsync(string userName, [FromBody] UserModel user)
  {
    var userId = (await userManager.FindByNameAsync(userName))?.Id;
    if (userId == null)
    {
      return new AjaxResp<UserModel>
      {
        Message = "未找到用户"
      };
    }


    await profileManager.AddProfilesAsync(userId.Value, user.Profiles);

    return new AjaxResp<UserModel>
    {
      Message = "保存成功",
      Data = user
    };
  }

  [HttpPost("/api/admin/users/{userName}/delete", Name = "管理员 - 停用用户")]
  public async Task<AjaxResp<bool>> AdminDeleteUserAsync(string userName)
  {
    var data = await userManager.SetDeletedAsync(userName, true);
    return new AjaxResp<bool> { Data = data };
  }

  [HttpPost("/api/admin/users/{userName}/active", Name = "管理员 - 激活用户")]
  public async Task<AjaxResp<bool>> AdminActiveUserAsync(string userName)
  {
    var data = await userManager.SetDeletedAsync(userName, false);
    return new AjaxResp<bool> { Data = data };
  }

  [HttpPost("/api/admin/users/{userName}/resetPassword", Name = "管理员 - 重置密码")]
  public async Task<AjaxResp<bool>> ResetPassword(string userName)
  {
    var data = await userManager.ResetPasswordAsync(userName);
    return new AjaxResp<bool> { Data = data };
  }

  [HttpGet("/api/admin/users/{userName}/isExist", Name = "管理员 - 判断用户是否存在")]
  public async Task<AjaxResp<bool>> AdminIsExistUserAsync(string userName)
  {
    var data = await userManager.IsExistUserAsync(userName);
    return new AjaxResp<bool> { Data = data };
  }

  [HttpPost("/api/admin/users/avatar", Name = "管理员 - 上传头像")]
  public AjaxResp AdminUploadAvatar(IFormFile avatar)
  {
    var data = profileManager.UploadImage(avatar);
    return new AjaxResp { Data = data };
  }

  [HttpGet("/api/admin/users/{userName}", Name = "管理员 - 用户基础信息")]
  public async Task<AjaxResp<UserModel>> GetBaseUserAsync(string userName)
  {
    var user = await userManager.FindByNameAsync(userName);
    var data = await userManager.GetBriefUserAsync(user!.Id, user.UserName!,
      new string[] { "public", "surname", "givenName" });
    return new AjaxResp<UserModel> { Data = data };
  }
}