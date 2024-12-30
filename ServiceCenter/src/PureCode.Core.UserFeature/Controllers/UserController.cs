using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PureCode.Core.Kernel;
using PureCode.Core.Managers;
using PureCode.Core.Models;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace PureCode.Core.Controllers;

[Route("/api/users", Name = "用户")]
//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = PermissionClaimNames.ApiPermission)]
[ApiController]
public partial class UserController : ControllerBase
{
  private readonly UserManager userManager;
  private readonly ProfileManager profileManager;

  public UserController(
    UserManager userManager,
    ProfileManager profileManager
  )
  {
    this.userManager = userManager;
    this.profileManager = profileManager;
  }

  [HttpGet(Name = "用户 - 搜索用户")]
  public async Task<PagedAjaxResp<UserModel>> FindUser(string? query = null, int page = 1, int size = 1000)
  {
    Dictionary<string, object?> dict;
    if (query != null)
    {
      dict = JsonSerializer.Deserialize<Dictionary<string, object>>(query) ?? new Dictionary<string, object?>();
    }
    else
    {
      dict = new Dictionary<string, object?>();
    }

    var total = await userManager.FindUsersCountAsync(dict);
    var users = await userManager.FindUsersAsync(dict, page, size);
    return new PagedAjaxResp<UserModel>
    {
      Code = 0,
      Total = total,
      Page = page,
      Data = users
    };
  }

  [HttpGet("/api/users/{userName}", Name = "用户 - 用户扼要信息")]
  public async Task<AjaxResp<UserModel>> GetBriefUserAsync(string userName)
  {
    var user = await userManager.FindByNameAsync(userName);

    if (user == null)
    {
      return new AjaxResp<UserModel>
      {
        Code = 404,
        Message = "无此用户信息"
      };
    }

    var data = await userManager.GetBriefUserAsync(user.Id, user.UserName!, new List<string>());
    return new AjaxResp<UserModel> { Data = data };
  }
}