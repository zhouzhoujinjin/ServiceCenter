using PureCode.ShortenUrl.Responses;
using System.Collections.Generic;

namespace PureCode.ShortenUrl.Requests
{
  public class CreateRequest : ShortenUrlRequest<CreateResponse>
  {
    public string Url { get; set; }
    public string? Keyword { get; set; }
    public string? Title { get; set; }

    public CreateRequest(string url)
    {
      Action = "shorturl";
      Url = url;
    }

    public override Dictionary<string, string> GetPostParameters()
    {
      var dict = base.GetPostParameters();
      dict["url"] = Url;
      if (!string.IsNullOrEmpty(Keyword))
      {
        dict["keyword"] = Keyword;
      }
      if (!string.IsNullOrEmpty(Title))
      {
        dict["title"] = Title;
      }
      return dict;
    }
  }
}