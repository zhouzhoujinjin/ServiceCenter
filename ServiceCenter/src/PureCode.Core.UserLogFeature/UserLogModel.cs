using System;
using System.Text.Json.Serialization;

namespace PureCode.Core.Models
{
  public class UserLogModel
  {
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("user")]
    public UserModel? User { get; set; }

    [JsonPropertyName("userId")]
    public ulong UserId { get; set; }

    [JsonPropertyName("createdTime")]
    public DateTime CreatedTime { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("ip")]
    public string? Ip { get; set; }

    [JsonPropertyName("device")]
    public string? Device { get; set; }

    [JsonPropertyName("method")]
    public string? Method { get; set; }

    [JsonPropertyName("userAgent")]
    public string? UserAgent { get; set; }

    [JsonPropertyName("data")]
    public string? Data { get; set; }

    [JsonPropertyName("duration")]
    public int Duration { get; set; }
  }
}