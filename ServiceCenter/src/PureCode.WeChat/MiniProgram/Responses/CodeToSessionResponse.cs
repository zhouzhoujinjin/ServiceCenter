using System.Text.Json.Serialization;

namespace PureCode.WeChat.MiniProgram.Responses
{
  public class CodeToSessionResponse : WeChatResponse
  {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    [JsonPropertyName("openid")]
    public string OpenId { get; set; }

    [JsonPropertyName("session_key")]
    public string SessionKey { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

    [JsonPropertyName("unionid")]
    public string? UnionId { get; set; }
  }
}