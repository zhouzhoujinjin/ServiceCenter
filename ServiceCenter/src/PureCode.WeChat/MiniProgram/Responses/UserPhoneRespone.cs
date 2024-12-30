using System.Text.Json.Serialization;

namespace PureCode.WeChat.MiniProgram.Responses
{
  public class UserPhoneResponse
  {
    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("purePhoneNumber")]
    public string? PurePhoneNumber { get; set; }

    [JsonPropertyName("countryCode")]
    public string? CountryCode { get; set; }

    [JsonPropertyName("watermark")]
    public Watermark? Watermark { get; set; }
  }
}