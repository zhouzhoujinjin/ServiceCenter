using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PureCode.Core.Converters
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class SensitiveDataAttribute : Attribute
  {
  }

  public abstract class SensitiveDataModel : IJsonOnSerialized, IJsonOnSerializing
  {
    private ConcurrentDictionary<string, object?> tempValues = new();

    public void OnSerializing()
    {
      var props = GetType().GetProperties().ToList();
      foreach (var prop in props)
      {
        var sensitiveDataAttributes = prop.GetCustomAttributes(false).FirstOrDefault(c => c is SensitiveDataAttribute);
        if (sensitiveDataAttributes != null)
        {
          tempValues[prop.Name] = prop.GetValue(this);
          prop.SetValue(this, prop.PropertyType.IsValueType ? Activator.CreateInstance(prop.PropertyType) : null);
        }
      }
    }

    public void OnSerialized()
    {
      var props = GetType().GetProperties().ToList();
      foreach (var prop in props)
      {
        var sensitiveDataAttributes = prop.GetCustomAttributes(false).FirstOrDefault(c => c is SensitiveDataAttribute);
        if (sensitiveDataAttributes != null)
        {
          prop.SetValue(this, tempValues[prop.Name]);
        }
      }
      tempValues.Clear();
    }
  }
}