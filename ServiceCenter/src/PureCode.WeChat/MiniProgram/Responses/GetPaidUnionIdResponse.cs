using System.Text.Json.Serialization;

namespace PureCode.WeChat.MiniProgram.Responses
{
  public class GetPaidUnionIdResponse : WeChatResponse
  {
    [JsonPropertyName("union_id")]
    public string? UnionId { get; set; }
  }
}