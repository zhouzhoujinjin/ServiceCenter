using System.Text.Json.Serialization;

namespace PureCode.WeChat.Work.Responses
{
  public class UploadMediaResponse : WeChatResponse
  {
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("media_id")]
    public string MediaId { get; set; }

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; }
  }

  public class DownloadMediaResponse : WeChatResponse
  {
    public string Type { get; set; }
    public string FileName { get; set; }
    public byte[] Stream { get; set; }
  }
}