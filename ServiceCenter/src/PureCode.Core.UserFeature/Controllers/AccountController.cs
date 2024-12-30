using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PureCode.Core.Kernel;
using PureCode.Core.Managers;
using PureCode.Core.Models;
using PureCode.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using PureCode.Core.UserFeature;
using PureCode.Core.UserFeature.Managers;
using PureCode.Core.UserFeature.Models;

namespace PureCode.Core.Controllers;

[ApiController]
[Route("/api/account", Name = "当前用户")]
public class AccountController(
  UserManager userManager,
  SignInManager signInManager,
  TokenManager tokenManager,
  RoleManager roleManager,
  CaptchaManager captchaManager,
  ProfileManager profileManager)
  : ControllerBase
{
  [HttpGet("captcha", Name = "获取验证码")]
  public async Task<AjaxResp<CaptchaModel>> GenerateCaptcha()
  {
    if (!captchaManager.Enabled)
    {
      return new AjaxResp<CaptchaModel>();
    }

    var data = await captchaManager.CreateCodeAsync();

    return new AjaxResp<CaptchaModel>
    {
      Data = data
    };
  }

  /// <summary>
  /// 用户使用用户名密码进行登录
  /// </summary>
  /// <param name="user">用户信息</param>
  /// <returns></returns>
  [HttpPost("login", Name = "用户 - 密码登录")]
  public async Task<AjaxResp<AuthenticationTokens>> LoginAsync([FromBody] LoginUserModel user)
  {
    if (captchaManager.Enabled)
    {
      var captchaCheckResultResult = await captchaManager.CheckCodeAsync(user.CaptchaId, user.CaptchaCode);
      if (captchaCheckResultResult != CaptchaCheckResult.Success)
      {
        return new AjaxResp<AuthenticationTokens>
        {
          Code = 400,
          Message = captchaCheckResultResult == CaptchaCheckResult.Failure ? "验证码错误" : "验证码过期"
        };
      }
    }

    var result = await signInManager.PasswordSignInAsync(user.UserName, user.Password, false, false);

    // 避免出现 Cookie 信息
    Response.Cookies.Delete(".AspNetCore.Identity.Application");
    Response.Headers.Remove("Set-Cookie");

    if (result.Succeeded)
    {
      var appUser = await userManager.FindByNameAsync(user.UserName);
      if (appUser != null)
        return new AjaxResp<AuthenticationTokens>()
        {
          Code = 0,
          Message = "登录成功",
          Data = await tokenManager.GenerateTokensAsync(appUser, user.RememberMe ?? false)
        };
    }

    return new AjaxResp<AuthenticationTokens>()
    {
      Code = 5001,
      Message = "登录失败",
      Data = null
    };
  }

  [HttpGet("profile", Name = "用户 - 用户资料")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
    Policy = PermissionClaimNames.ApiPermission)]
  public async Task<AjaxResp<UserModel>> GetProfileAsync()
  {
    return new AjaxResp<UserModel>
    {      
      Data = await userManager.GetBriefUserAsync(HttpContext.GetUserId(), HttpContext.GetUserName(),
        new[] { SystemProfileKeyCategory.Public })
    };
  }

  [HttpPost("profile", Name = "用户 - 更新资料")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
    Policy = PermissionClaimNames.ApiPermission)]
  public async Task<AjaxResp<UserModel>> UpdateProfileAsync([FromBody] Dictionary<string, JsonElement?> profiles)
  {
    await profileManager.AddProfilesAsync(HttpContext.GetUserId(), profiles);

    return new AjaxResp<UserModel>
    {
      Data = await userManager.GetBriefUserAsync(HttpContext.GetUserId(), null,
        new[] { SystemProfileKeyCategory.Public }),
      Message = "更新资料成功"
    };
  }

  [HttpPost("avatar", Name = "用户 - 更新头像")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
    Policy = PermissionClaimNames.ApiPermission)]
  public AjaxResp UploadAvatar(IFormFile avatar)
  {
    var data = profileManager.UploadImage(avatar);
    return new AjaxResp { Data = data };
  }

  [HttpGet("permissions", Name = "用户 - 权限列表")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
    Policy = PermissionClaimNames.ApiPermission)]
  public async Task<AjaxResp> GetPermissionsAsync()
  {
    var user = await userManager.GetUserAsync(HttpContext.User);

    if (user == null)
      return new AjaxResp
      {
        Code = 404,
        Message = "未找到用户信息"
      };
    var roles = await userManager.GetRolesAsync(user);
    var routePermissions = await roleManager.GetClaimsAsync(PermissionClaimNames.RoutePermission, roles);
    var actionPermissions = await roleManager.GetClaimsAsync(PermissionClaimNames.ActionPermission, roles);
    return new AjaxResp
    {
      Data = routePermissions.Concat(actionPermissions)
    };
  }

  [HttpPost("password", Name = "用户 - 更新密码")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
    Policy = PermissionClaimNames.ApiPermission)]
  public async Task<AjaxResp> UpdatePasswordAsync([FromBody] ChangePasswordModel passwords)
  {
    var user = await userManager.GetUserAsync(HttpContext.User);
    if (user != null)
    {
      var result = await userManager.ChangePasswordAsync(user, passwords.OldPassword, passwords.NewPassword);

      return new AjaxResp
      {
        Data = result.Succeeded,
        Message = result.Succeeded ? "修改密码成功" : "修改密码失败"
      };
    }
    else
    {
      return new AjaxResp
      {
        Code = 403,
        Message = "未找到登录用户"
      };
    }
  }
}