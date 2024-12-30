using PureCode.DataTransfer.Models;
using PureCode.DataTransfer.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PureCode.DataTransfer
{
  public static class ProviderExtensions
  {
    public static VerifyStatus LoadTemplate(this ISheetProvider provider, string templatePath, out IEnumerable<string> fieldTitles, out Dictionary<string, object> addonInfos)
    {
      using var stream = File.OpenRead(templatePath);
      return provider.LoadTemplate(stream, out fieldTitles, out addonInfos);
    }

    public static Stream FillData<T>(this ISheetProvider provider, string templatePath, IEnumerable<T> data, IEnumerable<Field> fields, Dictionary<string, object> addonInfos = null)
    {
      using var stream = File.OpenRead(templatePath);
      return provider.FillData(stream, typeof(T), data.Cast<object>(), fields, addonInfos);
    }

    public static Stream FillData<T>(this ISheetProvider provider, IEnumerable<T> data)
    {
      using var stream = new MemoryStream();
      var fields = provider.GetFields(typeof(T));
      return provider.FillData(stream, typeof(T), data.Cast<object>(), fields);
    }

    public static ICollection<object> LoadData(this ISheetProvider provider, string importFilePath, Type type, IEnumerable<Field> fields, Dictionary<string, object> addonInfos = null)
    {
      using var stream = File.OpenRead(importFilePath);
      return provider.LoadData(stream, type, fields, addonInfos) as ICollection<object>;
    }

    public static ICollection<T> LoadData<T>(this ISheetProvider provider, string importFilePath, IEnumerable<Field> fields, Dictionary<string, object> addonInfos = null) where T : new()
    {
      using var stream = File.OpenRead(importFilePath);
      return provider.LoadData(stream, typeof(T), fields, addonInfos).Cast<T>() as ICollection<T>;
    }

    public static ICollection<Dictionary<string, string>> LoadData(this ISheetProvider provider, string importFilePath, bool hasHeaderRow = true, IEnumerable<Field> fields = null)
    {
      using var stream = File.OpenRead(importFilePath);
      return provider.LoadData(stream, hasHeaderRow, fields);
    }
  }
}