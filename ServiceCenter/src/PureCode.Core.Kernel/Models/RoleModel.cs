using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PureCode.Core.Models
{
  public class RoleModel
  {
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("users")]
    public List<UserModel>? Users { get; set; }

    [JsonPropertyName("claims")]
    public List<string>? Claims { get; set; }
  }
}