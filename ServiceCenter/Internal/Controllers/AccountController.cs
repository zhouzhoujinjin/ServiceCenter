using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PureCode.Core;

namespace Internal.Controllers
{
  [ApiController]
  [Route("api/account", Name = "扩展 - 登录用户")]
  public class AccountController : ControllerBase
  {
#pragma warning disable IDE0052 // 删除未读的私有成员
    private readonly OpenIdConnectHandler _openIdConnectHandler;
#pragma warning restore IDE0052 // 删除未读的私有成员

    public AccountController(
        OpenIdConnectHandler openIdConnectHandler)
    {
      _openIdConnectHandler = openIdConnectHandler;
    }

    [Authorize(AuthenticationSchemes = "OpenIdConnect")]
    [HttpGet("oauth", Name = "OAuth 登录")]
    public AjaxResp<string> LoginByOAuth()
    {
      return new AjaxResp<string>();
    }

    [HttpGet("oauth-callback", Name = "OAuth 回调")]
    [Authorize(AuthenticationSchemes = "OpenIdConnect")]
    public AjaxResp<string> OAuthCallback()
    {
      return new AjaxResp<string>
      {
        Data = $"{User.Identity?.Name}"
      };
    }
  }
}