using System.Collections.Generic;

namespace PureCode.ShortenUrl
{
  public class ShortenUrlRequest<T> where T : ShortenUrlResponse
  {
    public string Action { get; internal set; }
    public string Format { get; } = "json";

    public string Token { get; set; }
    public long Timestamp { get; set; }

    public virtual Dictionary<string, string> GetPostParameters()
    {
      return new Dictionary<string, string>
      {
        ["action"] = Action,
        ["format"] = Format,
        ["timestamp"] = Timestamp.ToString(),
        ["signature"] = Token
      };
    }
  }
}