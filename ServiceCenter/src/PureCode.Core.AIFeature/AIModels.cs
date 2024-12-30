using System.Text.Json.Serialization;

namespace PureCode.Core.AIFeature
{
  public class AIRepuestModel : IAIRequest
  {
    [JsonPropertyName("model")]
    public required string Model { get; set; }
    [JsonPropertyName("prompt")]
    public required string Prompt { get; set; }
    [JsonPropertyName("stream")]
    public bool? Stream { get; set; }
    [JsonPropertyName("format")]
    public string? Format { get; set; }
    
  }

  public class AIReponseModel : IAIResponse
  {
    [JsonPropertyName("model")]
    public required string Model { get; set; }
    [JsonPropertyName("response")]
    public required string Response { get; set; }
  }
}
