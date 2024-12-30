using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PureCode.Core;
using PureCode.Core.Utils;
using PureCode.DataTransfer.Entities;
using PureCode.DataTransfer.Managers;
using PureCode.DataTransfer.Models;
using PureCode.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PureCode.DataTransfer.Controllers
{
  [Route("api/sheets")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public class SheetController : ControllerBase
  {
    private SheetManager sheetManager;
    private UploadOptions uploadOptions;

    public SheetController(
      SheetManager sheetManager,
      IOptions<UploadOptions> uploadOptionsAccessor
    )
    {
      this.sheetManager = sheetManager;
      uploadOptions = uploadOptionsAccessor.Value;
    }

    [HttpGet(Name = "表单列表")]
    public async Task<PagedAjaxResponse<FormEntity>> IndexAsync(int page = 1, int size = 20)
    {
      var result = await sheetManager.ListFormsAsync();
      return new PagedAjaxResponse<FormEntity>
      {
        Data = result.Skip(Math.Max(page - 1, 0) * size).Take(size),
        Page = Math.Max(page - 1, 0),
        Total = result.Count()
      };
    }

    [HttpGet("sources", Name = "表单数据源")]
    public async Task<AjaxResponse> GetDataSourcesAsync()
    {
      return new AjaxResponse
      {
        Data = await sheetManager.GetDataSourcesAsync()
      };
    }

    [HttpGet("{name}", Name = "表单详情")]
    public async Task<AjaxResponse<FormEntity>> GetForm(string name)
    {
      return new AjaxResponse<FormEntity>
      {
        Data = await sheetManager.GetSheetAsync(name)
      };
    }

    [HttpGet("sources/{name}", Name = "表单数据源域列表")]
    public async Task<AjaxResponse> GetFieldsAsync(string name)
    {
      return new AjaxResponse
      {
        Data = await sheetManager.GetDataFieldsAsync(name)
      };
    }

    [HttpPost(Name = "创建表单")]
    public async Task<AjaxResponse<FormEntity>> CreateAsync([FromBody] FormEntity form)
    {
      form = await sheetManager.CreateAsync(form);
      return new AjaxResponse<FormEntity>
      {
        Data = form
      };
    }

    [HttpPut("{name}", Name = "更新表单")]
    public async Task<AjaxResponse<FormEntity>> UpdateAsync(string name, [FromBody] FormEntity form)
    {
      form = await sheetManager.UpdateAsync(name, form);
      return new AjaxResponse<FormEntity>
      {
        Data = form
      };
    }

    [HttpPost("templates", Name = "上传模板")]
    public AjaxResponse UploadTemplate(IFormFile templatePath)
    {
      string rootPath = Path.Combine(uploadOptions.AbsolutePath, "sheets");
      string fileName = UploadUtils.MoveFile(templatePath, rootPath, false);
      if (string.IsNullOrEmpty(fileName))
      {
        return new AjaxResponse
        {
          Message = "上传模板失败"
        };
      }

      var webPath = UploadUtils.GetUrl(fileName, rootPath, uploadOptions.WebRoot + "/sheets");
      try
      {
        var (fields, addonInfos) = sheetManager.LoadTemplate(fileName);
        return new AjaxResponse
        {
          Data = new
          {
            TemplatePath = webPath,
            Fields = fields,
            AddonInofs = addonInfos
          }
        };
      }
      catch (Exception e)
      {
        return new AjaxResponse { Message = e.Message };
      }
    }

    [HttpPost("upload", Name = "上传数据")]
    public AjaxResponse UploadData(IFormFile importPath)
    {
      string rootPath = Path.Combine(uploadOptions.AbsolutePath);
      string fileName = UploadUtils.MoveFile(importPath, rootPath);
      if (string.IsNullOrEmpty(fileName))
      {
        return new AjaxResponse
        {
          Message = "上传数据失败"
        };
      }

      var webPath = UploadUtils.GetUrl(fileName, rootPath, uploadOptions.WebRoot);
      return new AjaxResponse
      {
        Data = webPath
      };
    }

    [HttpGet("{formName}/exists", Name = "判断表单是否存在")]
    public async Task<AjaxResponse<bool>> ExistsAsync(string formName, Dictionary<string, object> query)
    {
      return new AjaxResponse<bool>
      {
        Data = (await sheetManager.GetSheetAsync(formName)) != null
      };
    }

    [HttpPost("{formName}/export", Name = "导出预览")]
    public async Task<AjaxResponse<CacheEntity>> ExportPreviewAsync(string formName, Dictionary<string, object> query)
    {
      return null;
    }

    [HttpPost("{formName}/import", Name = "导入预览")]
    [Authorize(Policy = ClaimNames.ApiPermission)]
    public async Task<AjaxResponse> ImportPreviewAsync(string formName, [FromBody] ImportData importData)
    {
      var form = await sheetManager.GetSheetAsync(formName);
      importData.ImportPath = Path.Combine(uploadOptions.AbsolutePath, importData.ImportPath.StartsWith("/uploads/") ? importData.ImportPath.Substring(9) : importData.ImportPath);
      var cache = await sheetManager.SaveToCacheAsync(formName, importData.ImportPath, importData.Filters, HttpContext.GetUserId());
      return new AjaxResponse
      {
        Data = new
        {
          form.Fields,
          cache.Token,
          Cache = cache.Data.Take(10),
          Total = cache.Data.Count()
        }
      };
    }

    [HttpDelete("caches/{token}", Name = "清除缓存")]
    [Authorize(Policy = ClaimNames.ApiPermission)]
    public async Task<AjaxResponse> CleanCache(string token)
    {
      var success = (await sheetManager.RemoveCacheAsync(token)) > 0;
      return new AjaxResponse
      {
      };
    }
  }
}