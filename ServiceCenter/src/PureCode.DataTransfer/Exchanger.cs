using PureCode.DataTransfer.Models;
using PureCode.DataTransfer.Providers;
using System;
using System.Collections.Generic;
using System.IO;

namespace PureCode.DataTransfer
{
  public class Exchanger
  {
    private readonly ExcelProvider excelProvider;

    public Exchanger()
    {
      excelProvider = new ExcelProvider();
    }

    public VerifyStatus VerifyTemplate(string templatePath, out IEnumerable<string> fieldTitles, out Dictionary<string, object> addonInfos)
    {
      var provider = GetProvider(templatePath);
      return provider.LoadTemplate(templatePath, out fieldTitles, out addonInfos);
    }

    public IEnumerable<T> LoadData<T>(string importPath, IEnumerable<Field> fields, Dictionary<string, object> addonInfos = null) where T : new()
    {
      var provider = GetProvider(importPath);
      return provider.LoadData<T>(importPath, fields, addonInfos);
    }

    public IEnumerable<object> LoadData(string importPath, Type type, IEnumerable<Field> fields, Dictionary<string, object> addonInfos = null)
    {
      var provider = GetProvider(importPath);
      return provider.LoadData(importPath, type, fields, addonInfos);
    }

    public ICollection<Dictionary<string, string>> LoadData(string importPath, bool hasHeaderRow)
    {
      var provider = GetProvider(importPath);
      return provider.LoadData(importPath, hasHeaderRow);
    }

    private ISheetProvider GetProvider(string templatePath)
    {
      var extension = Path.GetExtension(templatePath).ToLower();
      if (extension == excelProvider.Extension)
      {
        return excelProvider;
      }

      return null;
    }
  }
}