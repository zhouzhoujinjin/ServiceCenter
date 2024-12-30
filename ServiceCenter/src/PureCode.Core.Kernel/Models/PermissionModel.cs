using System.Text.Json.Serialization;

namespace PureCode.Core.Models
{
  /// <summary>
  /// 权限点
  /// </summary>
  public class PermissionModel
  {
    /// <summary>
    /// 名称
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 值
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 组
    /// </summary>
    [JsonPropertyName("group")]
    public string Group { get; set; } = string.Empty;
  }
}