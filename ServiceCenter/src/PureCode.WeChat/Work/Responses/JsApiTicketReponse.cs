using System.Text.Json.Serialization;

namespace PureCode.WeChat.Work.Responses
{
  public class JsApiTicketReponse : WeChatResponse
  {
    [JsonPropertyName("ticket")]
    public string Ticket { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresTime { get; set; }
  }
}