using System.Text.Json.Serialization;

namespace PureCode.WeChat.Work.Responses
{
  public class MessageResponse : WeChatResponse
  {
    [JsonPropertyName("invaliduser")]
    public string InvalidUser { get; set; }

    [JsonPropertyName("invalidparty")]
    public string InvalidParty { get; set; }

    [JsonPropertyName("invalidtag")]
    public string InvalidTag { get; set; }
  }
}