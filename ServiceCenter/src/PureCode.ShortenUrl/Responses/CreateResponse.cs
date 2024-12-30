using System.Text.Json.Serialization;

namespace PureCode.ShortenUrl.Responses
{
  public class ShortenUrlUrl
  {
    [JsonPropertyName("keyword")]
    public string Keyword { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("date")]
    public string CreatedTime { get; set; }
  }

  public class CreateResponse : ShortenUrlResponse
  {
    [JsonPropertyName("url")]
    public ShortenUrlUrl Url { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("shorturl")]
    public string ShortUrl { get; set; }
  }
}