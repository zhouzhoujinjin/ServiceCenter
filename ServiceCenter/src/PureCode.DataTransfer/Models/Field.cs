using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PureCode.DataTransfer.Models
{
  public class Field
  {
    [JsonPropertyName("source")]
    public string Source { get; set; }

    [JsonPropertyName("format")]
    public string Format { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("indices")]
    public List<string> Indices { get; set; }
  }

  public class DataField
  {
    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("children")]
    public List<DataField> Children { get; set; }
  }
}