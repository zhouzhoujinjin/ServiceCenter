using System.Collections.Generic;

namespace PureCode.DataTransfer.Models
{
  public class PreviewData<T>
  {
    public string FileLink { get; set; }
    public Dictionary<string, string> AddonInfos { get; set; }
    public int TotalCount { get; set; }
    public int PreviewCount { get; set; }
    public IEnumerable<T> Data { get; set; }
  }
}