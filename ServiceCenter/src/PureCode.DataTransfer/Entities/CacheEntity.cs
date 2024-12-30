using System;
using System.Collections.Generic;

namespace PureCode.DataTransfer.Entities
{
  public class CacheEntity
  {
    public long Id { get; set; }
    public int FormId { get; set; }
    public string Token { get; set; } = string.Empty;
    public FormEntity? Form { get; set; }
    public Dictionary<string, object> Filters { get; set; } = new Dictionary<string, object>();
    public List<object> Data { get; set; } = new List<object>();
    public string? FilePath { get; set; }
    public long CreatorId { get; set; }
    public DateTime CreatedTime { get; set; }
  }
}