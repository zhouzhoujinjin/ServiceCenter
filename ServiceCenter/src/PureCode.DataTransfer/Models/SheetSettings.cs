using System.Collections.Generic;

namespace PureCode.DataTransfer.Models
{
  public class SheetSettings
  {
    public string TemplatePath { get; set; }
    public IEnumerable<string> Params { get; set; }
    public int DataStartRow { get; set; }
    public int DataStartCol { get; set; }
    public IEnumerable<Field> Fields { get; set; }
  }
}