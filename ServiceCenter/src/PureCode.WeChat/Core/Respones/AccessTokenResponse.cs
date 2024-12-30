using System.Text.Json.Serialization;

namespace PureCode.WeChat.Responses
{
  public class AccessTokenResponse : WeChatResponse
  {
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
  }
}