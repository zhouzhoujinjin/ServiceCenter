using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PureCode.Core.Kernel;
using PureCode.Core.Managers;
using PureCode.Core.Models;
using System.Collections.Generic;

namespace PureCode.Core.Controllers
{
  [Route("api/valueSpaces", Name = "值空间")]
  [ApiController]
  //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public partial class ValueSpaceController : ControllerBase
  {
    private readonly ValueSpaceManager valueSpaceManager;

    public ValueSpaceController(ValueSpaceManager valueSpaceManager)
    {
      this.valueSpaceManager = valueSpaceManager;
    }

    /// <summary>
    /// 获取所有的值空间列表
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "值空间字典")]
    //[Authorize(Policy = PermissionClaimNames.ApiPermission)]
    public AjaxResp<SortedDictionary<string, ValueSpaceModel>> Get()
    {
      var rsp = valueSpaceManager.GetVsMap();
      return new AjaxResp<SortedDictionary<string, ValueSpaceModel>>
      {
        Data = rsp
      };
    }

    /// <summary>
    /// 根据名称获得指定的值空间
    /// </summary>
    /// <param name="name">值空间名称</param>
    /// <returns></returns>
    [HttpGet("{name}", Name = "值空间详情")]
    public AjaxResp<ValueSpaceModel> Get(string name)
    {
      var result = valueSpaceManager.GetVsMap().TryGetValue(name, out var valueSpace);
      if (result)
      {
        return new AjaxResp<ValueSpaceModel>
        {
          Data = valueSpace
        };
      }
      else
      {
        return new AjaxResp<ValueSpaceModel>
        {
          Code = 404,
          Message = "未找到",
        };
      }
    }
  }
}