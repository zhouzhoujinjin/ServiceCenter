using System;
using System.Collections.Generic;
using System.Text.Json;

namespace PureCode.WeChat
{
  public class WeChatDictionary : SortedDictionary<string, string?>
  {
    public WeChatDictionary()
    {
    }

    public WeChatDictionary(IDictionary<string, string?> dictionary)
        : base(dictionary)
    { }

    public void Add(string key, int value)
    {
      Add(key, value.ToString());
    }

    public void Add(string key, uint value)
    {
      Add(key, value.ToString());
    }

    public void Add(string key, long value)
    {
      Add(key, value.ToString());
    }

    public new void Add(string key, string? value)
    {
      if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
      {
        base.Add(key, value);
      }
    }

    public void Add<T>(string key, T value)
    {
      if (Nullable.GetUnderlyingType(typeof(T)) != null)
      {
        if (value != null)
        {
          base.Add(key, value.ToString());
        }
      }
      else
      {
        // hack the value is an object to json string
        base.Add(key, "@@" + JsonSerializer.Serialize(value) + "@@");
      }
    }

    public string GetValue(string key)
    {
      return TryGetValue(key, out var o) ? o : null;
    }

    public string SetValue(string key, string value)
    {
      return this[key] = value;
    }
  }
}