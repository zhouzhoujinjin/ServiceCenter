using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PureCode.WeChat.MiniProgram
{
  public class SubscribeMessageValue
  {
    [JsonPropertyName("value")]
    public string Value { get; set; }
  }

  public class SubscribeMessageData : Dictionary<string, SubscribeMessageValue>
  {
    public SubscribeMessageData()
    { }

    public SubscribeMessageData(IDictionary<string, string> data)
    {
      foreach (var kv in data)
      {
        Add(kv.Key, new SubscribeMessageValue { Value = kv.Value });
      }
    }
  }

  public enum MiniProgramState
  {
    Formal,
    Developer,
    Trial
  }
}

namespace PureCode.WeChat.MiniProgram.Requests
{
  public class SendSubscribeMessageRequest : WeChatRequest<WeChatResponse>
  {
    public override HttpMethod RequestMethod => HttpMethod.POST;

    public string? ToUser { get; set; }
    public string? TemplateId { get; set; }
    public string Page { get; set; }
    public SubscribeMessageData Data { get; set; }
    public MiniProgramState MiniProgramState { get; set; }

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
        { WeChatConsts.ToUser, ToUser },
        { WeChatConsts.TemplateId, TemplateId },
        { WeChatConsts.Page, Page },
        { WeChatConsts.MiniProgramState, nameof(MiniProgramState).ToLower() }
      };
      pp.Add(WeChatConsts.Data, Data);
      return pp;
    }
  }
}