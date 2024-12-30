using System.Text.Json.Serialization;

namespace PureCode.Core.Models
{
  public class ChangePasswordModel
  {
    [JsonPropertyName("oldPassword")]
    public required string OldPassword { get; set; }

    [JsonPropertyName("newPassword")]
    public required string NewPassword { get; set; }
  }
}