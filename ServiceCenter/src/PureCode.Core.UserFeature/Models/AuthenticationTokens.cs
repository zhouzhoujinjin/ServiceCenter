using System.Text.Json.Serialization;

namespace PureCode.Core.Models
{
  public class AuthenticationTokens
  {
    /// <summary>
    /// 访问票据
    /// </summary>
    [JsonPropertyName("accessToken")]
    public required string AccessToken { get; set; }

    /// <summary>
    /// 刷新票据
    /// </summary>
    [JsonPropertyName("refreshToken")]
    public string? RefreshToken { get; set; }
  }
}