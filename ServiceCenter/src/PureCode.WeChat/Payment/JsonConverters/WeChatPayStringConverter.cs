using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PureCode.WeChat.Payment.JsonConverters
{
  /// <summary>
  /// 将 JsonTokenType.Number、JsonTokenType.Array 转为 JsonString
  /// </summary>
  public class WeChatPayStringConverter : JsonConverter<string>
  {
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      switch (reader)
      {
        case { TokenType: JsonTokenType.Number } when reader.TryGetInt64(out var int64):
          {
            return int64.ToString();
          }
        case { TokenType: JsonTokenType.Number }:
          {
            return reader.GetDouble().ToString(CultureInfo.InvariantCulture);
          }
        case { TokenType: JsonTokenType.StartArray }:
          {
            var node = System.Text.Json.Nodes.JsonNode.Parse(ref reader);
            return node?.ToJsonString();
          }
        default:
          return reader.GetString();
      }
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
      if (value == null)
      {
        writer.WriteNullValue();
      }
      else
      {
        writer.WriteStringValue(value.AsSpan());
      }
    }
  }
}