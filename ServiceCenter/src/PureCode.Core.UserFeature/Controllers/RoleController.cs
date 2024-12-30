using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PureCode.Core.Kernel;
using PureCode.Core.Managers;
using PureCode.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PureCode.Core.Controllers
{
  //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = PermissionClaimNames.ApiPermission)]
  [Route("/api/roles", Name = "角色")]
  public partial class RoleController
  {
    private readonly UserManager userManager;
    private readonly RoleManager roleManager;

    public RoleController(RoleManager roleManager, UserManager userManager)
    {
      this.userManager = userManager;
      this.roleManager = roleManager;
    }

    [HttpGet(Name = "用户 - 角色扼要列表")]
    public async Task<AjaxResp<IEnumerable<RoleModel>>> ListBriefRolesAsync()
    {
      return new()
      {
        Data = (await roleManager.GetBriefRolesAsync()).Select(x => new RoleModel
        {
          Name = x.Key,
          Title = x.Value
        }).OrderBy(x => x.Name),
      };
    }
  }
}