using PureCode.Security;
using PureCode.Utils;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

namespace PureCode.WeChat.Work.Requests
{
  public class Article
  {
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("picurl")]
    public string Picurl { get; set; }
  }

  public class SendWebhookMarkdownRequest : WeChatRequest<WeChatResponse>
  {
    public string Key { get; set; }
    public string Content { get; set; }
    public override HttpMethod RequestMethod => HttpMethod.POST;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/webhook/send";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { "key", Key }
      };
      return qs;
    }

    public override WeChatDictionary GetPostParameters()
    {
      var pp = new WeChatDictionary
      {
        { "msgtype", "markdown" },
        { "markdown", new Dictionary<string, string> { { "content", Content } } }
      };
      return pp;
    }
  }

  public class SendWebhookImageRequest : WeChatRequest<WeChatResponse>
  {
    public string Key { get; set; }
    public string ImagePath { get; set; }
    public override HttpMethod RequestMethod => HttpMethod.POST;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/webhook/send";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { "key", Key }
      };
      return qs;
    }

    public override WeChatDictionary GetPostParameters()
    {
      FileStream file = new FileStream(ImagePath, FileMode.Open);
      var md5 = MD5.Compute(file, false);
      var base64 = file.ToBase64();

      var pp = new WeChatDictionary
      {
        { "msgtype", "image" },
        { "image", new Dictionary<string, string> { { "base64", base64 }, { "md5", md5 } } }
      };
      return pp;
    }
  }

  public class SendWebhookNewsRequest : WeChatRequest<WeChatResponse>
  {
    public string Key { get; set; }
    public List<Article> Articles { get; set; }

    public override HttpMethod RequestMethod => HttpMethod.POST;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/webhook/send";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { "key", Key }
      };
      return qs;
    }

    public override WeChatDictionary GetPostParameters()
    {
      var pp = new WeChatDictionary
      {
        { "msgtype", "news" },
        { "news", new Dictionary<string, List<Article>> { { "articles", Articles } } }
      };
      return pp;
    }
  }
}