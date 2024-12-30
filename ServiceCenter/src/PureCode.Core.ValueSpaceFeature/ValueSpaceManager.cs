using Microsoft.EntityFrameworkCore;
using PureCode.Core.Entities;
using PureCode.Core.Models;

namespace PureCode.Core.Managers
{
  public class ValueSpaceManager
  {
    private readonly object _locker = new();

    public ValueSpaceManager(PureCodeDbContext context)
    {
      this.context = context;
      valueSpaceSet = context.Set<ValueSpaceEntity>();
    }

    internal async Task InitializeAsync()
    {
      var values = await valueSpaceSet.ToListAsync();
      var map = new SortedDictionary<string, ValueSpaceModel>();
      foreach (var val in values)
      {
        switch (val.ValueSpaceType)
        {
          case ValueSpaceType.Code:
            map[val.Name] = new CodeValueSpaceModel(val.Name, val.Title, val.ConfigureLevel, ParseCodes(val));
            break;

          case ValueSpaceType.Range:
            map[val.Name] = new RangeValueSpaceModel(val.Name, val.Title, val.ConfigureLevel, ParseRanges(val));
            break;

          case ValueSpaceType.Regex:
            map[val.Name] = new RegexValueSpaceModel(val.Name, val.Title, val.ConfigureLevel, ParseRegexPatterns(val));
            break;
        }
      }

      lock (_locker)
      {
        ValueSpaceMap.Value = map;
      }
    }

    internal static ValueSpaceMap ValueSpaceMap { get; private set; } = new ValueSpaceMap();
    private readonly PureCodeDbContext context;
    private readonly DbSet<ValueSpaceEntity> valueSpaceSet;

    public SortedDictionary<string, ValueSpaceModel> GetVsMap() => ValueSpaceMap.Value;

    public async Task<ValueSpaceEntity?> GetByNameAsync(string name)
    {
      return await valueSpaceSet.FirstOrDefaultAsync(r => r.Name == name);
    }

    public async Task<ValueSpaceEntity?> GetByIdAsync(ulong id)
    {
      return await valueSpaceSet.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task SaveAsync(ValueSpaceModel dto)
    {
      var vs = await GetByNameAsync(dto.Name) ?? throw new PureCodeException($"未找到 [{dto.Name}] 的值空间");
      vs.Title = dto.Title;
      vs.ValueSpaceType = dto.ValueSpaceType;

      vs.Items = SerializeItems(dto);
      valueSpaceSet.Update(vs);
      await context.SaveChangesAsync();
      await InitializeAsync();
    }

    public async Task AddAsync(ValueSpaceModel dto)
    {
      var vs = new ValueSpaceEntity
      {
        Name = dto.Name,
        Title = dto.Title,
        ValueSpaceType = dto.ValueSpaceType,
        ConfigureLevel = dto.ConfigureLevel,
        Items = SerializeItems(dto)
      };

      valueSpaceSet.Add(vs);
      await context.SaveChangesAsync();
      await InitializeAsync();
    }

    public async Task DeleteAsync(ValueSpaceEntity vs)
    {
      valueSpaceSet.Remove(vs);
      await context.SaveChangesAsync();
      await InitializeAsync();
    }

    private static Dictionary<string, string> ParseCodes(ValueSpaceEntity vs)
    {
      var items = vs.Items.Split('\n');
      var cvs = new Dictionary<string, string>();
      foreach (var item in items)
      {
        if (string.IsNullOrEmpty(item))
        {
          continue;
        }

        var cv = item.Trim().Split(':');
        cvs.Add(cv[0], cv.Length == 1 ? cv[0] : cv[1]);
      }

      return cvs;
    }

    private static Dictionary<string, float> ParseRanges(ValueSpaceEntity vs)
    {
      var items = vs.Items.Split('\n');
      var ranges = new Dictionary<string, float>();
      foreach (var item in items)
      {
        if (string.IsNullOrEmpty(item))
        {
          continue;
        }

        var parts = item.Trim().Split(":");
        string range, title;
        if (parts.Length == 1)
        {
          if (string.IsNullOrEmpty(parts[0])) continue;
          range = title = parts[0];
        }
        else
        {
          if (string.IsNullOrEmpty(parts[1])) continue;
          title = parts[0];
          range = parts[1];
        }

        if (float.TryParse(range, out var v))
        {
          ranges.Add(title, v);
        }
      }

      return ranges;
    }

    private static List<string> ParseRegexPatterns(ValueSpaceEntity vs)
    {
      var items = vs.Items.Split('\n').ToList();
      return items;
    }

    private static string SerializeItems(ValueSpaceModel dto)
    {
      switch (dto.ValueSpaceType)
      {
        case ValueSpaceType.Code:
          var codeStr = string.Empty;
          var kv = dto.Conditions as Dictionary<string, string>;
          foreach (var (key, value) in kv!)
          {
            if (string.IsNullOrEmpty(key.Trim()))
            {
              continue;
            }

            codeStr += key + ":" + value + "\n";
          }

          return codeStr[..^1];

        case ValueSpaceType.Range:
          var rangeStr = string.Empty;
          var ranges = dto.Conditions as Dictionary<string, float>;
          foreach (var i in ranges!)
          {
            if (string.IsNullOrEmpty(i.Key.Trim()))
            {
              continue;
            }

            rangeStr += i.Key + ":" + i.Value.ToString("0.00") + "\n";
          }

          return rangeStr[..^1];

        case ValueSpaceType.Regex:
          return string.Join("\n", dto.GetItemNames());

        default:
          return "";
      }
    }
  }
}