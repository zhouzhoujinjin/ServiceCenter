using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PureCode.DataTransfer.Entities;
using PureCode.DataTransfer.Models;
using PureCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PureCode.DataTransfer.Managers
{
  public class SheetManager
  {
    private SheetContext context;
    private Exchanger exchanger;
    private readonly IDistributedCache cache;

    public SheetManager(
      SheetContext context,
      Exchanger exchanger,
      IDistributedCache cache
    )
    {
      this.context = context;
      this.exchanger = exchanger;
      this.cache = cache;
    }

    public async Task<IEnumerable<FormEntity>> ListFormsAsync()
    {
      var forms = await context.FormSet.ToListAsync();

      return forms;
    }

    public async Task<FormEntity> GetSheetAsync(string name)
    {
      return await context.FormSet.FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task<object> GetDataSourcesAsync()
    {
      var all = await LoadAllExchangerAsync();

      return all.Select(x => Type.GetType(x)).Select(
        x => new
        {
          x.GetCustomAttribute<ExchangerAttribute>().Title,
          ClassName = x.FullName,
          Filters = x.GetProperties()
                     .Where(x => x.GetCustomAttribute<FilterAttribute>() != null)?
                     .ToDictionary(x => x.Name, x => x.GetCustomAttribute<FilterAttribute>().Title)
        }
      );
    }

    private async Task<IEnumerable<string>> LoadAllExchangerAsync()
    {
      return await cache.GetAsync("SHEET_DATA_SOURCES", () =>
      {
        var ie = typeof(IExchanger);
        var entryAssembly = Assembly
          .GetEntryAssembly();
        var allAssembley = entryAssembly.GetReferencedAssemblies()
          .Where(x => !x.FullName.StartsWith("Microsoft"))
          .ToList();
        allAssembley.Add(entryAssembly.GetName());
        var all = allAssembley.Select(x => Assembly.Load(x))
          .SelectMany(x => x.DefinedTypes).Where(type =>
          {
            var result = type != ie && ie.IsAssignableFrom(type);
            return result;
          }).Where(x => x.GetCustomAttribute<ExchangerAttribute>() != null);
        return Task.FromResult(all.Select(t => t.AssemblyQualifiedName).ToArray());
      },
      new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) });
    }

    private List<DataField> GetSheetFields(Type type, HashSet<string> hashSet = null)
    {
      if (hashSet == null)
      {
        hashSet = new HashSet<string>();
      }

      var fields = new List<DataField>();
      foreach (var pi in type.GetProperties())
      {
        var displayAttr = pi.GetCustomAttribute<FieldAttribute>();
        if (displayAttr != null && !hashSet.Contains(pi.PropertyType.AssemblyQualifiedName))
        {
          var field = new DataField
          {
            Value = pi.Name,
            Title = displayAttr.Label
          };

          if (pi.PropertyType.IsGenericType)
          {
            field.Value += ".";
            field.Children = GetSheetFields(pi.PropertyType.GenericTypeArguments[0], hashSet);
          }
          else if (!pi.PropertyType.IsPrimitive && pi.PropertyType != typeof(string) && pi.PropertyType != typeof(DateTime))
          {
            hashSet.Add(pi.PropertyType.AssemblyQualifiedName);
            field.Children = GetSheetFields(pi.PropertyType, hashSet);
          }
          fields.Add(field);
        }
      }
      return fields;
    }

    public async Task<IEnumerable<DataField>> GetDataFieldsAsync(string name)
    {
      try
      {
        var types = await LoadAllExchangerAsync();
        var typeName = types.Where(t => t.StartsWith(name)).FirstOrDefault();
        if (typeName != null)
        {
          var type = Type.GetType(typeName);
          return GetSheetFields(type);
        }
      }
      catch
      {
      }
      return null;
    }

    public (IEnumerable<string> fieldTitles, Dictionary<string, object> addonInfos) LoadTemplate(string templatePath)
    {
      var status = exchanger.VerifyTemplate(templatePath, out var fieldTitles, out var addonInfos);
      switch (status)
      {
        case VerifyStatus.Success:
          return (fieldTitles, addonInfos);

        case VerifyStatus.DuplicatedProperties:
          throw new Exception("包含重复的属性");
        case VerifyStatus.TemplateNotSupported:
          throw new Exception("系统不支持此模板，如左侧列包含数据");
        case VerifyStatus.Failed:
          throw new Exception("未知错误");
      }
      return (null, null);
    }

    public async Task<FormEntity> CreateAsync(FormEntity form)
    {
      context.Add(form);
      await context.SaveChangesAsync();
      return form;
    }

    public async Task<FormEntity> UpdateAsync(string name, FormEntity form)
    {
      var old = context.FormSet.Where(x => x.Name == name).FirstOrDefault();
      if (old == null)
      {
        old = new FormEntity();
        context.Add(old);
      }
      old.Fields = form.Fields;
      old.DataType = form.DataType;
      old.AddonInfos = form.AddonInfos;
      old.Filters = form.Filters;
      old.Name = form.Name;
      old.TemplatePath = form.TemplatePath;
      old.Title = form.Title;
      if (old.Id != default)
      {
        context.Update(old);
      }
      await context.SaveChangesAsync();
      return form;
    }

    public async Task<CacheEntity> SaveToCacheAsync(
      string formName, string filePath, Dictionary<string, object> filters, long creatorId
    )
    {
      var form = await context.FormSet.Where(x => x.Name == formName).FirstOrDefaultAsync();
      if (form == null)
      {
        return null;
      }
      var all = await LoadAllExchangerAsync();
      var formDataType = all.Select(x => Type.GetType(x)).Where(x => x.FullName == form.DataType).FirstOrDefault();

      var result = exchanger.LoadData(filePath, formDataType, form.Fields, form.AddonInfos).ToList();

      CacheEntity cache = new CacheEntity
      {
        Token = DateTime.Now.Ticks.ToString().ComputeMd5(),
        Filters = filters,
        FormId = form.Id,
        Data = result,
        CreatorId = creatorId,
        CreatedTime = DateTime.Now
      };
      context.Add(cache);
      await context.SaveChangesAsync();
      return cache;
    }

    public async Task<CacheEntity> GetCacheAsync(string token)
    {
      return await context.DataSet.Include(x => x.Form).Where(x => x.Token == token).FirstOrDefaultAsync();
    }

    public async Task<int> RemoveCacheAsync(string token)
    {
      var cache = context.DataSet.Where(x => x.Token == token);
      context.DataSet.RemoveRange(cache);
      return await context.SaveChangesAsync();
    }
  }
}