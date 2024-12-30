using System.Text.Json.Serialization;

namespace PureCode.WeChat
{
  public class WeChatResponse
  {
    [JsonIgnore]
    public string AppId { get; set; }

    [JsonPropertyName("errcode")]
    public int ErrorCode { get; set; }

    [JsonPropertyName("errmsg")]
    public string ErrorMessage { get; set; } = "";

    [JsonIgnore]
    public object? Addon { get; set; }
  }
}