using System.Collections.Generic;

namespace PureCode.WeChat.Payment
{
  public class WeChatPayDictionary : SortedDictionary<string, string>
  {
    public WeChatPayDictionary()
    { }

    public WeChatPayDictionary(IDictionary<string, string> dictionary)
        : base(dictionary)
    { }

    public void Add(string key, object value)
    {
      string strValue;

      if (value == null)
      {
        return;
      }
      else if (value is string str)
      {
        strValue = str;
      }
      else if (value is int?)
      {
        strValue = (value as int?).Value.ToString();
      }
      else if (value is long?)
      {
        strValue = (value as long?).Value.ToString();
      }
      else if (value is double?)
      {
        strValue = (value as double?).Value.ToString();
      }
      else if (value is bool?)
      {
        strValue = (value as bool?).Value.ToString().ToLowerInvariant();
      }
      else
      {
        strValue = value.ToString();
      }

      if (!string.IsNullOrEmpty(strValue))
      {
        base.Add(key, strValue);
      }
    }

    public string? GetValue(string key)
    {
      return TryGetValue(key, out var o) ? o : null;
    }

    public string SetValue(string key, string value)
    {
      return this[key] = value;
    }
  }
}