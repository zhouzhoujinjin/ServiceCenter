using PureCode.Core.Services;

namespace TestUtilityFeature
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public void TestWordExport()
    {
      var service = new WordExportService();
      service.ExportTemplate("item-template.docx", "output.docx", new Dictionary<string, string>
      {
        {"a", "a" },
        {"b", "b"},
        {"c", "c"},
      });
    }

    [TestMethod]
    public void TestExcelExport()
    {
      var service = new ExcelExportService();
      service.ExportList("export.xlsx", new List<List<string>> { new List<string> { "a", "b", "c" }, new List<string> { "a", "b", "c" } });
    }
  }
}