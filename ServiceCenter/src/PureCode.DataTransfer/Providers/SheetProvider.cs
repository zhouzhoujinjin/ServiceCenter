using PureCode.DataTransfer.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PureCode.DataTransfer.Providers
{
  public abstract class SheetProvider : ISheetProvider
  {
    public virtual string Extension { get; } = "";

    public virtual Stream FillData(Stream templateStream, Type dataType, IEnumerable<object> data, IEnumerable<Field> fields, Dictionary<string, object> addonInfos = null) => throw new NotImplementedException();

    public virtual ICollection<object> LoadData(Stream input, Type dataType, IEnumerable<Field> fields, Dictionary<string, object> addonInfos = null) => throw new NotImplementedException();

    public virtual ICollection<Dictionary<string, string>> LoadData(Stream input, bool hasTitleRow = true, IEnumerable<Field> fields = null) => throw new NotImplementedException();

    public virtual VerifyStatus LoadTemplate(Stream fileStream, out IEnumerable<string> fieldTitles, out Dictionary<string, object> addonInfos) => throw new NotImplementedException();

    public virtual IEnumerable<Field> GetFields(Type type, Field parent = null)
    {
      var fields = new List<Field>();
      foreach (var pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite && !p.PropertyType.IsGenericType))
      {
        var field = new Field
        {
          Source = parent == null ? pi.Name : $"{parent.Source}.{pi.Name}",
        };
        var fieldAttr = pi.GetCustomAttribute<FieldAttribute>();
        field.Title = fieldAttr == null ? parent?.Title + pi.Name : parent?.Title + fieldAttr.Label;
        if (pi.PropertyType.IsPrimitive || pi.PropertyType == typeof(string))
        {
          fields.Add(field);
        }
        else if (pi.PropertyType == typeof(DateTime))
        {
          field.Format = Format.DateWithDash;
          fields.Add(field);
        }
        else
        {
          fields.AddRange(GetFields(pi.PropertyType, field));
        }
      }
      return fields;
    }

    protected static object GetValue(Type type, object value, Field field)
    {
      var parts = field.Source.Split(".");
      foreach (var part in parts)
      {
        if (value == null)
        {
          return null;
        }
        var r = int.TryParse(part, out var index);
        if (r)
        {
          var enumerator = (value as IEnumerable)?.GetEnumerator();
          if (enumerator == null)
          {
            return null;
          }
          var j = -1;
          while (j < index)
          {
            enumerator.MoveNext();
            j++;
          }
          value = enumerator.Current;
          if (value != null) type = value.GetType();
        }
        else
        {
          var pi = type.GetProperties().FirstOrDefault(x => string.Equals(x.Name, part, StringComparison.CurrentCultureIgnoreCase));
          if (pi != null)
          {
            value = pi.GetValue(value);
            type = value.GetType();
          }
          else
          {
            return null;
          }
        }
      }
      return value;
    }

    protected virtual void SetValue(object entry, object value, Field field)
    {
      var parts = field.Source.Split(".");
      var parent = entry;
      foreach (var part in parts)
      {
        var type = parent.GetType();
        if (type.IsGenericType)
        {
          type = type.GenericTypeArguments[0];
        }
        var pi = type.GetProperties().FirstOrDefault(x => string.Equals(x.Name, part, StringComparison.CurrentCultureIgnoreCase));
        if (pi != null && (pi.PropertyType.IsPrimitive || pi.PropertyType == typeof(string)))
        {
          pi.SetValue(parent, value.ToString());
        }
        else if (pi != null && pi.PropertyType == typeof(DateTime))
        {
          pi.SetValue(parent, DateTime.ParseExact(value.ToString(), field.Format, null));
        }
        else
        {
          if (int.TryParse(part, out var index))
          {
            var list = (parent as IList);
            while (list.Count <= index)
            {
              list.Add(Activator.CreateInstance(type));
            }
            parent = list[index];
          }
          else if (pi != null)
          {
            var data = pi.GetValue(parent);

            if (data == null)
            {
              data = Activator.CreateInstance(pi.PropertyType);
              pi.SetValue(parent, data);
            }
            parent = data;
          }
        }
      }
    }
  }
}