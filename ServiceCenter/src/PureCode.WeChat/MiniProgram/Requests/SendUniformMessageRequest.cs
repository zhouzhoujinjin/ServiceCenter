using PureCode.WeChat.MiniProgram.Responses;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PureCode.WeChat.MiniProgram
{
  public class UniformMessageValue
  {
    [JsonPropertyName("value")]
    public string? Value { get; set; }
  }

  public class UniformMessageData : Dictionary<string, UniformMessageValue>
  {
    public UniformMessageData()
    {
    }

    public UniformMessageData(IDictionary<string, string> data)
    {
      foreach (var kv in data)
      {
        Add(kv.Key, new UniformMessageValue { Value = kv.Value });
      }
    }
  }
}

namespace PureCode.WeChat.MiniProgram.Requests
{
  public class SendUniformMessageRequest : WeChatRequest<SendUniformMessageResponse>
  {
    public override HttpMethod RequestMethod => HttpMethod.POST;

    public string? ToUser { get; set; }
    public string? TemplateId { get; set; }
    public string? FormId { get; set; }
    public string? Page { get; set; }
    public string? EmphasisKeyword { get; set; }
    public UniformMessageData Data { get; set; }

    public override string CommandUrl => "https://api.weixin.qq.com/cgi-bin/message/subscribe/send";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChatConsts.AccessToken, context.AccessToken }
      };
      return qs;
    }

    public override WeChatDictionary GetPostParameters()
    {
      var pp = new WeChatDictionary
      {
        { WeChatConsts.ToUser, ToUser }
      };
      var dict = new WeChatDictionary
      {
        {WeChatConsts.TemplateId, TemplateId },
        {WeChatConsts.FormId, FormId },
        {WeChatConsts.Page, Page },
        {WeChatConsts.EmphasisKeyword,  EmphasisKeyword}
      };
      dict.Add(WeChatConsts.Data, Data);
      pp.Add(WeChatConsts.MiniProgramTemplateMessage, dict);
      return pp;
    }
  }
}