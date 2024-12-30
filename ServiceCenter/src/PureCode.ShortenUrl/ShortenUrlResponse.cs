using System.Text.Json.Serialization;

namespace PureCode.ShortenUrl
{
  public class ShortenUrlResponse
  {
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }
  }
}