using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PureCode.Core.Kernel;
using PureCode.Core.Models;

namespace PureCode.Core.Controllers
{
  public partial class ValueSpaceController
  {
    [HttpPost("/api/admin/valueSpaces/{name}/update", Name = "更新值空间")]
    //[Authorize(Policy = PermissionClaimNames.ApiPermission)]
    public async Task<AjaxResp<ValueSpaceModel>> UpdateAsync(string name, [FromBody] ValueSpaceModel valueSpace)
    {
      var result = valueSpaceManager.GetVsMap().TryGetValue(name, out var vs);
      if (!result)
      {
        return new AjaxResp<ValueSpaceModel>
        {
          Code = 404,
          Message = $"找不到 {name} 值空间"
        };
      }

      if (vs != null)
      {
        vs.Name = name;
        vs.Title = valueSpace.Title;
        vs.Conditions = valueSpace.Conditions;

        await valueSpaceManager.SaveAsync(vs);
      }

      return new AjaxResp<ValueSpaceModel>
      {
        Code = 0,
        Data = vs,
        Message="更新成功"
      };
    }

    [HttpGet("/api/admin/valueSpaces", Name = "值空间摘要列表")]
    [Authorize(Policy = PermissionClaimNames.ApiPermission)]
    public PagedAjaxResp<ValueSpaceModel> Index(int page = 1, int size = 10)
    {
      var valueSpaces = valueSpaceManager.GetVsMap()
        .Select(v => v.Value)
        .OrderBy(v => v.Name)
        .Skip(Math.Max(page - 1, 0) * size)
        .Take(size).Select(v =>
        {
          if (v.ValueSpaceType == ValueSpaceType.Code)
          {
            return new CodeValueSpaceModel(
              v.Name, v.Title, v.ConfigureLevel,
              ((Dictionary<string, string>)v.Conditions!).Take(6));
          }
          else
          {
            return v;
          }
        });
      return new PagedAjaxResp<ValueSpaceModel>
      {
        Data = valueSpaces,
        Total = valueSpaceManager.GetVsMap().Count,
        Page = page
      };
    }
  }
}