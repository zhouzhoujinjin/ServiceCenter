using System.Text.Json.Serialization;

namespace PureCode.WeChat.Work.Responses
{
  public class CodeToUserResponse : WeChatResponse
  {
    [JsonPropertyName("UserId")]
    public string UserId { get; set; }

    [JsonPropertyName("OpenId")]
    public string OpenId { get; set; }

    [JsonPropertyName("DeviceId")]
    public string DeviceId { get; set; }
  }
}