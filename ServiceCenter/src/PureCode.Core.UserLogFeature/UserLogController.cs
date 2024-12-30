using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PureCode.Core.Kernel;
using PureCode.Core.Managers;
using PureCode.Core.Models;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace PureCode.Core.Controllers
{
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = PermissionClaimNames.ApiPermission)]
  [Route("/api/admin/userLogs")]
  public class UserLogController : ControllerBase
  {
    private readonly UserLogManager userLogManager;

    public UserLogController(UserLogManager userLogManager)
    {
      this.userLogManager = userLogManager;
    }

    [HttpGet(Name = "日志列表")]
    public async Task<PagedAjaxResp<UserLogModel>> IndexAsync(string? query = null, int page = 1, int size = 20)
    {
      var dict = query != null
            ? JsonSerializer.Deserialize<Dictionary<string, string>>(query)!
            : new Dictionary<string, string>();

      var (list, total) = await userLogManager.ListUserLogsAsync(dict, page, size);
      return PagedAjaxResp<UserLogModel>.Create(list, total, page, size);
    }
  }
}