using PureCode.WeChat.MiniProgram.Responses;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PureCode.WeChat.MiniProgram.Requests
{
  public class GetMiniProgramCodeRequest : WeChatRequest<MiniProgramCodeResponse>
  {
    [JsonPropertyName("scene")]
    public string? Scene { get; set; }

    [JsonPropertyName("page")]
    public string? Page { get; set; }

    [JsonPropertyName("Width")]
    public string? Width { get; set; }

    [JsonPropertyName("auto_color")]
    public bool AutoColor { get; set; }

    [JsonPropertyName("line_color")]
    public Dictionary<string, string>? LineColor { get; set; }

    [JsonPropertyName("is_hyaline")]
    public bool BackgroundIsTransparent { get; set; }
  }
}