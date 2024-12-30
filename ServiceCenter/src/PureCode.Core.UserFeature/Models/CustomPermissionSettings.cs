using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PureCode.Core.Models
{
  public class CustomPermission
  {
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
  }

  public class CustomPermissionSettings
  {
    [JsonPropertyName("values")]
    public List<CustomPermission>? Values { get; set; }
  }
}