using System.Collections.Generic;

namespace PureCode.DataTransfer.Models
{
  public class ImportData
  {
    public string ImportPath { get; set; }
    public Dictionary<string, object> Filters { get; set; }
  }
}