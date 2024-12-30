using PureCode.Core.Kernel;
using System.Text.Json.Serialization;

namespace PureCode.Core.Models
{
  /// <summary>
  /// 资料点
  /// </summary>
  public class ProfileKeyModel
  {
    /// <summary>
    /// 名称
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("isPublic")]
    public bool IsPublic { get; set; }

    [JsonPropertyName("isBrief")]
    public bool IsBrief { get; set; }

    [JsonPropertyName("searchable")]
    public bool Searchable { get; set; }

    [JsonPropertyName("valueSpaceName")]
    public string? ValueSpaceName { get; set; }

    [JsonPropertyName("className")]
    public string ProfileTypeClassName { get; set; } = DefaultProfileKeyClassTypes.String;

    [JsonIgnore]
    public Type? ProfileType { get; set; }

    /// <summary>
    /// 分类代码
    /// </summary>
    [JsonPropertyName("categoryCode")]
    public string? CategoryCode { get; set; }
  }
}