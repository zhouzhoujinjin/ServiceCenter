using PureCode.Core.Models;
using PureCode.Utils;

namespace PureCode.Core
{
  public class ValueSpaceMap
  {
    public SortedDictionary<string, ValueSpaceModel> Value { get; set; } = new SortedDictionary<string, ValueSpaceModel>();

    /// <summary>
    /// 代码类型的值空间获得标题对应的代码
    /// </summary>
    /// <param name="name"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    public string? GetCodeByNameTitle(string name, string title)
    {
      Value.TryGetValue(name, out var vs);
      if (vs == null || vs.ValueSpaceType != ValueSpaceType.Code) return "";
      var dictionary = vs.Conditions as Dictionary<string, string>;
      var kv = dictionary?.FirstOrDefault(kv => kv.Value == title);
      return kv?.Key;
    }

    /// <summary>
    /// 代码类型的值空间获得代码对应的标题
    /// </summary>
    /// <param name="name"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public string? GetTitleByNameKey(string name, object key)
    {
      var title = string.Empty;
      name = name.ToCamelCase();
      Value.TryGetValue(name, out var vs);
      if (vs is { ValueSpaceType: ValueSpaceType.Code } && key != null)
      {
        ((Dictionary<string, string>)vs.Conditions!)?.TryGetValue(key.ToString()!, out title);
      }

      return title;
    }

    /// <summary>
    /// 范围类型的值空间获得值对应的标题
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public string? GetTitleByNameValue(string name, float value)
    {
      string title = string.Empty;
      Value.TryGetValue(name, out var vs);
      if (vs == null || vs.ValueSpaceType != ValueSpaceType.Range) return title;
      var range = vs as RangeValueSpaceModel;
      return range?.TitleOf(value);
    }

    public ValueSpaceModel Get(string name)
    {
      return Value[name];
    }

    public T Get<T>(string name) where T : ValueSpaceModel
    {
      return (T)Value[name];
    }
  }
}