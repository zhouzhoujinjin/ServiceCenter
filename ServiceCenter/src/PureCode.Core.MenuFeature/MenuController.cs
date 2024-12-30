using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PureCode.Core.Kernel;
using PureCode.Core.Managers;
using PureCode.Core.Models;

namespace PureCode.Core.Controllers;

[ApiController]
[Route("", Name = "菜单")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = PermissionClaimNames.ApiPermission)]
public partial class MenuController(MenuManager menuManager, UserManager userManager,RoleManager roleManager) : ControllerBase
{
  [HttpGet("/api/account/nav", Name = "用户 - 导航菜单")]
  public async Task<AjaxResp<ICollection<MenuItemModel>?>> GetMainMenu()
  {
    var menu = await menuManager.GetMenuAsync();

    var user = await userManager.GetUserAsync(HttpContext.User);
    if (user != null)
    {
      var userMenu = await menuManager.FilterMenusAsync(menu, user);
      return new AjaxResp<ICollection<MenuItemModel>?> { Data = userMenu };
    }
    else
    {
      return new AjaxResp<ICollection<MenuItemModel>?> { Code = 404, Message = "未找到用户信息 [导航菜单]" };
    }
  }
}