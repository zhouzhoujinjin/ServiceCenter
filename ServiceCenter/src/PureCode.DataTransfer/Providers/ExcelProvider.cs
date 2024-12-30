using Aspose.Cells;
using PureCode.DataTransfer.Models;
using PureCode.DataTransfer.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PureCode.DataTransfer.Providers
{
  public class ExcelProvider : SheetProvider
  {
    public override string Extension => ".xlsx";

    public override Stream FillData(Stream templateStream, Type dataType, IEnumerable<object> data, IEnumerable<Field> fields, Dictionary<string, object>? addonInfos = null)
    {
      try
      {
        using var workbook = new Workbook(templateStream);
        if (workbook.Worksheets.Count == 0)
        {
          workbook.Worksheets.Add("Sheet 1");
        }
        var sheet = workbook.Worksheets[0];
        var startRow = sheet.Cells.MaxRow + 1;
        var startCol = 1;
        if (addonInfos != null)
        {
          var item = addonInfos.FirstOrDefault(x => string.Equals(x.Key, "StartRow", StringComparison.CurrentCultureIgnoreCase));
          if (!item.Equals(new KeyValuePair<string, object>()))
          {
            int.TryParse(item.Value.ToString(), out startRow);
          }
          item = addonInfos.FirstOrDefault(x => string.Equals(x.Key, "StartCol", StringComparison.CurrentCultureIgnoreCase));
          if (!item.Equals(new KeyValuePair<string, object>()))
          {
            int.TryParse(item.Value.ToString(), out startCol);
          }
        }
        else if (startRow == 1)
        {
          var col = 1;
          foreach (var field in fields)
          {
            sheet.Cells[1, col].Value = field.Title;
            col++;
          }
          startRow = 2;
        }
        var row = startRow;
        foreach (var item in data)
        {
          var col = startCol;
          foreach (var field in fields)
          {
            var value = item is IDictionary dict ? dict[field.Source] : GetValue(dataType, item, field);
            if (field.Format != null)
            {
              switch (field.Format)
              {
                case Format.Date:
                case Format.DateWithDash:
                case Format.YearAndMonth:
                case Format.YearAndMonthWithDash:
                  value = (value as DateTime?)?.ToString(Format.Date);
                  break;

                case Format.ExcelDateTime:
                  value = DateTimeUtils.ToExcelSerialDate(value as DateTime?);
                  sheet.Cells[row, col].GetStyle().Custom = "yyyy-MM-dd";
                  break;

                case Format.EmbedImage:
                  break;
              }
            }
            sheet.Cells[row, col].Value = value;
            col++;
          }
          row++;
        }

        return workbook.SaveToStream();
      }
      catch (Exception e)
      {
        return null;
      }
    }

    public override VerifyStatus LoadTemplate(Stream fileStream, out IEnumerable<string> fieldTitles, out Dictionary<string, object> addonInfos)
    {
      try
      {
        using var workbook = new Workbook(fileStream);
        var sheet = workbook.Worksheets[0];

        var startEmptyColumn = 1;
        while (sheet.Cells[sheet.Cells.MaxRow + 1, startEmptyColumn].Value != null)
        {
          startEmptyColumn++;
        }
        fieldTitles = GetFieldTitles(sheet);
        if (fieldTitles == null)
        {
          addonInfos = null;
          return VerifyStatus.TemplateNotSupported;
        }
        addonInfos = new Dictionary<string, object> {
          {"StartRow", sheet.Cells.MaxRow + 1 },
          {"StartColumn", startEmptyColumn }
        };
        return VerifyStatus.Success;
      }
      catch (Exception)
      {
        fieldTitles = null;
        addonInfos = null;
        return VerifyStatus.Failed;
      }
    }

    public override ICollection<object> LoadData(Stream inputStream, Type dataType, IEnumerable<Field> fields, Dictionary<string, object> addonInfos = null)
    {
      using (var workbook = new Workbook(inputStream))
      {
        var sheet = workbook.Worksheets[0];
        var startRow = sheet.Cells.MaxRow + 1;
        var startCol = 1;
        if (addonInfos != null)
        {
          var item = addonInfos.FirstOrDefault(x => string.Equals(x.Key, "StartRow", StringComparison.CurrentCultureIgnoreCase));
          if (!item.Equals(new KeyValuePair<string, object>()))
          {
            int.TryParse(item.Value.ToString(), out startRow);
          }
          item = addonInfos.FirstOrDefault(x => string.Equals(x.Key, "StartCol", StringComparison.CurrentCultureIgnoreCase));
          if (!item.Equals(new KeyValuePair<string, object>()))
          {
            int.TryParse(item.Value.ToString(), out startCol);
          }
        }

        var listType = typeof(List<>);
        var constructedListType = listType.MakeGenericType(typeof(object));

        var entries = Activator.CreateInstance(constructedListType) as List<object>;

        for (var row = startRow; row <= sheet.Cells.MaxRow; row++)
        {
          var entry = Activator.CreateInstance(dataType);
          var index = 0;
          foreach (var field in fields)
          {
            var value = sheet.Cells[row, startCol + index].Value;
            if (value != null)
            {
              SetValue(entry, value, field);
            }
            index++;
          }
          entries.Add(entry);
        }
        return entries;
      }
    }

    public override ICollection<Dictionary<string, string>> LoadData(Stream input, bool hasTitleRow = true, IEnumerable<Field> fields = null)
    {
      using var workbook = new Workbook(input);
      var sheet = workbook.Worksheets[0];
      var startRow = hasTitleRow ? 2 : 1;
      var startCol = 1;
      var entries = new List<Dictionary<string, string>>();

      if (fields == null)
      {
        fields = new List<Field>();
        for (var i = 1; i < sheet.Cells.MaxRow + 1; i++)
        {
          fields.Append(new Field
          {
            Title = hasTitleRow ? sheet.Cells[1, i].StringValue : ""
          });
        }
      }

      for (var row = startRow; row <= sheet.Cells.MaxRow; row++)
      {
        var entry = new Dictionary<string, string>();
        var index = 0;
        foreach (var field in fields)
        {
          entry[field.Source ?? field.Title] = sheet.Cells[row, startCol + index].StringValue;
          index++;
        }
        entries.Add(entry);
      }
      return entries;
    }

    private IEnumerable<string> GetFieldTitles(Worksheet sheet, int? headerEndRow = null)
    {
      var fieldTitles = Enumerable.Repeat(string.Empty, sheet.Cells.MaxColumn).ToArray();
      if (headerEndRow == null)
      {
        headerEndRow = sheet.Cells.MaxRow;
      }
      for (var i = 1; i < headerEndRow + 1; i++)
      {
        var leftEmptyTextColumnCounts = Enumerable.Repeat(0, sheet.Cells.MaxColumn).ToArray();
        var mergedCells = sheet.Cells.MergedCells.ToArray().Select(x => (CellArea)x);
        for (var j = 1; j < sheet.Cells.MaxColumn + 1; j++)
        {
          var range = mergedCells.Where(x => x.StartRow == i && x.StartColumn == j).FirstOrDefault();
          if (range.Equals(default(CellArea)))
          {
            if (sheet.Cells[i, j].Value != null)
            {
              fieldTitles[j - 1] += sheet.Cells[i, j].StringValue;
            }
            else
            {
              leftEmptyTextColumnCounts[j - 1] = j - 1 == 0 ? 0 : leftEmptyTextColumnCounts[j - 2] + 1;
            }
          }
          else
          {
            fieldTitles[j - 1] = sheet.Cells[range.StartRow, range.StartColumn].StringValue;
          }
        }
        var increasing = true;
        for (var j = sheet.Cells.MaxColumn - 1; j > 1; j--)
        {
          if (leftEmptyTextColumnCounts[j] < leftEmptyTextColumnCounts[j - 1])
          {
            increasing = false;
          }
        }

        if (increasing && leftEmptyTextColumnCounts.Count(v => v == 0) != leftEmptyTextColumnCounts.Length)
        {
          return null;
        }
      }
      return fieldTitles;
    }
  }
}