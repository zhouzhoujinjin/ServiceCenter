using Aspose.Words;
using Aspose.Words.MailMerging;
using System.Data;
using System.Text;

namespace PureCode.Core.Services
{
  public class WordExportService
  {
    public WordExportService()
    {
      new License().SetLicense(
        new MemoryStream(Convert.FromBase64String("PExpY2Vuc2U+DQogIDxEYXRhPg0KICAgIDxMaWNlbnNlZFRvPkREVFdDPC9MaWNlbnNlZFRvPg0KICAgIDxFbWFpbFRvPmluZm9AZGV2ZXNwcml0LmNvbTwvRW1haWxUbz4NCiAgICA8TGljZW5zZVR5cGU+U2l0ZSBPRU08L0xpY2Vuc2VUeXBlPg0KICAgIDxMaWNlbnNlTm90ZT5VcCBUbyAxMCBEZXZlbG9wZXJzIEFuZCBVbmxpbWl0ZWQgRGVwbG95bWVudCBMb2NhdGlvbnM8L0xpY2Vuc2VOb3RlPg0KICAgIDxPcmRlcklEPjIyMDgxNzE2NTg1NjwvT3JkZXJJRD4NCiAgICA8VXNlcklEPjgyODI1MTwvVXNlcklEPg0KICAgIDxPRU0+VGhpcyBpcyBhIHJlZGlzdHJpYnV0YWJsZSBsaWNlbnNlPC9PRU0+DQogICAgPFByb2R1Y3RzPg0KICAgICAgPFByb2R1Y3Q+QXNwb3NlLlRvdGFsIGZvciAuTkVUPC9Qcm9kdWN0Pg0KICAgIDwvUHJvZHVjdHM+DQogICAgPEVkaXRpb25UeXBlPkVudGVycHJpc2U8L0VkaXRpb25UeXBlPg0KICAgIDxTZXJpYWxOdW1iZXI+YjNkOTRmYzUtZWVhYi00NzU2LWI0NjUtZDE3NTgwNTRlZWZkPC9TZXJpYWxOdW1iZXI+DQogICAgPFN1YnNjcmlwdGlvbkV4cGlyeT4yMDI0MDExNjwvU3Vic2NyaXB0aW9uRXhwaXJ5Pg0KICAgIDxMaWNlbnNlRXhwaXJ5PjIwMjMwMjE2PC9MaWNlbnNlRXhwaXJ5Pg0KICAgIDxFeHBpcnlOb3RlPlRoaXMgaXMgYSB0ZW1wb3JhcnkgbGljZW5zZSBmb3Igbm9uLWNvbW1lcmNpYWwgdXNlIG9ubHkgYW5kIGl0IHdpbGwgZXhwaXJlIG9uIDIwMjMtMDItMTY8L0V4cGlyeU5vdGU+DQogICAgPExpY2Vuc2VWZXJzaW9uPjMuMDwvTGljZW5zZVZlcnNpb24+DQogICAgPExpY2Vuc2VJbnN0cnVjdGlvbnM+aHR0cHM6Ly9wdXJjaGFzZS5hc3Bvc2UuY29tL3BvbGljaWVzL3VzZS1saWNlbnNlPC9MaWNlbnNlSW5zdHJ1Y3Rpb25zPg0KICA8L0RhdGE+DQogIDxTaWduYXR1cmU+QlAzTFRxdnRLN0FjQjU2R2swb05nLyt3YkVDSmp1S0Z3WGRnU2NsR2xTYkV0ZUpsZi9lekhoc0FaR2dBMUxmQ0xKS0p0OWpzZzlkZmlUc0p3Zm56cmc1RVFoNExvdjFVR2pTUDhGV2ZFU25YNGFlSXFwdlRBaXgwV0dvbGtsVnMxak5PMitBK1Z6Rzc1TDRJK2pmdTByOWpKQk1tQ1ZJZjMrMWpLWndoeDA0PTwvU2lnbmF0dXJlPg0KPC9MaWNlbnNlPg==")));
    }

    public Document GetDocument()
    {
      return new Document();
    }

    public Document GetDocument(string path)
    {
      return new Document(path);
    }

    /// <summary>
    /// 使用 Word MergeField （合并域）功能进行数据填充。
    /// 创建好模板docx，设置对应的MergeField（在 [插入] -> [文档部件] -> [域...] 中选择 [MergeField]，设置域名）。
    /// 如果要插入图片将域名设置为 `Image:域名` 的格式。
    /// </summary>
    /// <param name="templatePath">模板路径，可以为绝对路径，或相对于主程序的相对路径。</param>
    /// <param name="outputPath">输出路径，可以为绝对路径，或相对于主程序的相对路径。可以为doc、docx、pdf。</param>
    /// <param name="fields">传入的域字典。注意如果是要插入图片仅将key设为域名，不要包含 `Image:`。</param>
    /// <returns></returns>
    public void ExportTemplate(string templatePath, string outputPath, Dictionary<string, string> fields)
    {
      var document = new Document(templatePath);
      var keys = fields.Keys.ToArray();
      var values = fields.Values.ToArray();
      document.MailMerge.FieldMergingCallback = new HandleMergeImageField();
      document.MailMerge.Execute(keys, values);
      document.Save(outputPath);
    }

    /// <summary>
    /// 根据模板样式导出列表数据。
    /// 循环体需包裹在域 `TableStart:${listName}` 和域 `TableEnd:${listName}`之间
    /// </summary>
    /// <param name="templatePath">模板路径，可以为绝对路径，或相对于主程序的相对路径。</param>
    /// <param name="listName">模板中的循环列表名称</param>
    /// <param name="outputPath">输出路径，可以为绝对路径，或相对于主程序的相对路径。可以为doc、docx、pdf。</param>
    /// <param name="array">数据列表，列表元素为传入的域字典。注意如果是要插入图片仅将key设为域名，不要包含 `Image:`。</param>
    public void ExportList(string templatePath, string listName, string outputPath, List<Dictionary<string, string>> array)
    {
      var document = new Document(templatePath);
      var keys = array.First().Keys.ToArray();
      document.MailMerge.FieldMergingCallback = new HandleMergeImageField();

      var dataTable = new DataTable(listName);
      dataTable.Columns.AddRange(keys.Select(x => new DataColumn(x)).ToArray());
      array.ForEach(x => dataTable.Rows.Add(x.Values));
      document.MailMerge.ExecuteWithRegions(dataTable);
      document.Save(outputPath);
    }

    private class HandleMergeImageField : IFieldMergingCallback
    {
      public void FieldMerging(FieldMergingArgs args)
      {
      }

      public void ImageFieldMerging(ImageFieldMergingArgs args)
      {
        var path = args.FieldValue.ToString();
        if (path == null)
        {
          return;
        }
        using var fileStream = File.OpenRead(path);
        var memoryStream = new MemoryStream();
        fileStream.CopyTo(memoryStream);
        memoryStream.Position = 0;
        args.ImageStream = memoryStream;
      }
    }
  }
}