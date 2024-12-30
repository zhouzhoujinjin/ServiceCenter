using System.Text.Json.Serialization;

namespace PureCode.WeChat.MiniProgram.Responses
{
  public class Watermark
  {
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    [JsonPropertyName("timestamp")]
    public int Timestamp { get; set; }
  }

  public class UserInfoResponse
  {
    [JsonPropertyName("nickName")]
    public string? NickName { get; set; }

    [JsonPropertyName("gender")]
    public int Gender { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("province")]
    public string? Province { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("avatarUrl")]
    public string? AvatarUrl { get; set; }

    [JsonPropertyName("openId")]
    public string? OpenId { get; set; }

    [JsonPropertyName("unionId")]
    public string? UnionId { get; set; }

    [JsonPropertyName("watermark")]
    public Watermark? Watermark { get; set; }
  }
}