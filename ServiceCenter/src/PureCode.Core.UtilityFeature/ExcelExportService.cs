using Aspose.Cells;
using PureCode.Utils;
using System.Text;

namespace PureCode.Core.Services
{
  public class ExcelExportService
  {
    public ExcelExportService()
    {
      new License().SetLicense(
        new MemoryStream(Encoding.UTF8.GetBytes("""
          <?xml version="1.0"?>
          <License>
            <Data>
              <LicensedTo>DDTWC</LicensedTo>
              <EmailTo>info@devesprit.com</EmailTo>
              <LicenseType>Site OEM</LicenseType>
              <LicenseNote>Up To 10 Developers And Unlimited Deployment Locations</LicenseNote>
              <OrderID>220817165856</OrderID>
              <UserID>828251</UserID>
              <OEM>This is a redistributable license</OEM>
              <Products>
                <Product>Aspose.Total for .NET</Product>
              </Products>
              <EditionType>Enterprise</EditionType>
              <SerialNumber>b3d94fc5-eeab-4756-b465-d1758054eefd</SerialNumber>
              <SubscriptionExpiry>20240116</SubscriptionExpiry>
              <LicenseExpiry>20230216</LicenseExpiry>
              <ExpiryNote>This is a temporary license for non-commercial use only and it will expire on 2023-02-16</ExpiryNote>
              <LicenseVersion>3.0</LicenseVersion>
              <LicenseInstructions>https://purchase.aspose.com/policies/use-license</LicenseInstructions>
            </Data>
            <Signature>BP3LTqvtK7AcB56Gk0oNg/+wbECJjuKFwXdgSclGlSbEteJlf/ezHhsAZGgA1LfCLJKJt9jsg9dfiTsJwfnzrg5EQh4Lov1UGjSP8FWfESnX4aeIqpvTAix0WGolklVs1jNO2+A+VzG75L4I+jfu0r9jJBMmCVIf3+1jKZwhx04=</Signature>
          </License>
          """)));
    }

    public Workbook GetWorkbook()
    {
      return new Workbook();
    }

    public Workbook GetWorkbook(string path)
    {
      return new Workbook(path);
    }

    /// <summary>
    /// 根据模板样式导出列表数据。
    /// 循环体需包裹在域 `TableStart:${listName}` 和域 `TableEnd:${listName}`之间
    /// </summary>
    /// <param name="templatePath">模板路径，可以为绝对路径，或相对于主程序的相对路径。</param>
    /// <param name="listName">模板中的循环列表名称</param>
    /// <param name="outputPath">输出路径，可以为绝对路径，或相对于主程序的相对路径。可以为doc、docx、pdf。</param>
    /// <param name="array">数据列表，列表元素为传入的域字典。注意如果是要插入图片仅将key设为域名，不要包含 `Image:`。</param>
    public void ExportList(string outputPath, IEnumerable<IEnumerable<string>> array)
    {
      var workbook = new Workbook();
      var sheet = workbook.Worksheets[0];

      array.ForEach((item, row) =>
      {
        item.ForEach((cell, col) =>
        {
          sheet.Cells[row, col].PutValue(cell);
        });
      });
      workbook.Save(outputPath);
    }
  }
}