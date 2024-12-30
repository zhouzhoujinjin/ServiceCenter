using PureCode.WeChat.Work.Responses;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PureCode.WeChat.Work.Requests
{
  public interface IMessagePostEntity
  {
    string ToUser { get; set; }
    string ToParty { get; set; }
    string ToTag { get; set; }
    string MsgType { get; }
    int AgentId { get; set; }
    int Safe { get; set; }
    int EnableDuplicateCheck { get; set; }
    int DuplicateCheckInterval { get; set; }
  }

  public class News
  {
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("picurl")]
    public string PicUrl { get; set; }
  }

  public class MpNews
  {
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("thumb_media_id")]
    public string ThumbMediaId { get; set; }

    [JsonPropertyName("author")]
    public string Author { get; set; }

    [JsonPropertyName("content_source_url")]
    public string ContentSourceUrl { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("digest")]
    public string Digest { get; set; }
  }

  public class ContentItem
  {
    [JsonPropertyName("key")]
    public string Key { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }
  }

  public class MiniProgramNotice
  {
    [JsonPropertyName("appid")]
    public string AppId { get; set; }

    [JsonPropertyName("page")]
    public string Page { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("emphasis_first_item")]
    public bool EmphasisFirstItem { get; set; }

    [JsonPropertyName("content_item")]
    public ICollection<ContentItem> ContentItems { get; set; }
  }

  public class TaskCardBtn
  {
    [JsonPropertyName("key")]
    public string Key { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("replace_name")]
    public string ReplaceName { get; set; }

    [JsonPropertyName("color")]
    public string Color { get; set; }

    [JsonPropertyName("is_bold")]
    public bool IsBold { get; set; }
  }

  public class TaskCard
  {
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("task_id")]
    public string TaskId { get; set; }

    [JsonPropertyName("btn")]
    public ICollection<TaskCardBtn> Btn { get; set; }
  }

  public class SendTextMessageRequest : WeChatRequest<MessageResponse>, IMessagePostEntity
  {
    public string ToUser { get; set; }
    public string ToParty { get; set; }
    public string ToTag { get; set; }
    public string MsgType
    { get { return "text"; } }
    public int AgentId { get; set; }
    public int Safe { get; set; } = 0;
    public int EnableDuplicateCheck { get; set; } = 0;
    public int DuplicateCheckInterval { get; set; } = 1800;

    public int EnableIdTrans { get; set; } = 0;
    public string Content { get; set; }

    public override HttpMethod RequestMethod => HttpMethod.POST;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/message/send";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken }
      };
      return qs;
    }

    public override WeChatDictionary GetPostParameters()
    {
      var pp = new WeChatDictionary
      {
        { "touser", ToUser },
        { "toparty", ToParty },
        { "totag", ToTag },
        { "msgtype", MsgType },
        { "agentid", AgentId },
        { "text", new Dictionary<string, string> { { "content", Content } } },
        { "safe", Safe },
        { "enable_id_trans", EnableIdTrans },
        { "enable_duplicate_check", EnableDuplicateCheck },
        { "duplicate_check_interval", DuplicateCheckInterval }
      };
      return pp;
    }
  }

  public class SendImageMessageRequest : WeChatRequest<MessageResponse>, IMessagePostEntity
  {
    public string ToUser { get; set; }
    public string ToParty { get; set; }
    public string ToTag { get; set; }
    public string MsgType
    { get { return "image"; } }
    public int AgentId { get; set; }
    public int Safe { get; set; } = 0;
    public int EnableDuplicateCheck { get; set; } = 0;
    public int DuplicateCheckInterval { get; set; } = 1800;

    public string MediaId { get; set; }

    public override HttpMethod RequestMethod => HttpMethod.POST;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/message/send";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken }
      };
      return qs;
    }

    public override WeChatDictionary GetPostParameters()
    {
      var pp = new WeChatDictionary
      {
        { "touser", ToUser },
        { "toparty", ToParty },
        { "totag", ToTag },
        { "msgtype", MsgType },
        { "agentid", AgentId },
        { "image", new Dictionary<string, string> { { "media_id", MediaId } } },
        { "safe", Safe },
        { "enable_duplicate_check", EnableDuplicateCheck },
        { "duplicate_check_interval", DuplicateCheckInterval }
      };
      return pp;
    }
  }

  public class SendVoiceMessageRequest : WeChatRequest<MessageResponse>, IMessagePostEntity
  {
    public string ToUser { get; set; }
    public string ToParty { get; set; }
    public string ToTag { get; set; }
    public string MsgType
    { get { return "voice"; } }
    public int AgentId { get; set; }
    public int Safe { get; set; } = 0;
    public int EnableDuplicateCheck { get; set; } = 0;
    public int DuplicateCheckInterval { get; set; } = 1800;

    public string MediaId { get; set; }

    public override HttpMethod RequestMethod => HttpMethod.POST;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/message/send";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken }
      };
      return qs;
    }

    public override WeChatDictionary GetPostParameters()
    {
      var pp = new WeChatDictionary
      {
        { "touser", ToUser },
        { "toparty", ToParty },
        { "totag", ToTag },
        { "msgtype", MsgType },
        { "agentid", AgentId },
        { "voice", new Dictionary<string, string> { { "media_id", MediaId } } },
        { "safe", Safe },
        { "enable_duplicate_check", EnableDuplicateCheck },
        { "duplicate_check_interval", DuplicateCheckInterval }
      };
      return pp;
    }
  }

  public class SendVideoMessageRequest : WeChatRequest<MessageResponse>, IMessagePostEntity
  {
    public string ToUser { get; set; }
    public string ToParty { get; set; }
    public string ToTag { get; set; }
    public string MsgType
    { get { return "video"; } }
    public int AgentId { get; set; }
    public int Safe { get; set; } = 0;
    public int EnableDuplicateCheck { get; set; } = 0;
    public int DuplicateCheckInterval { get; set; } = 1800;

    public string MediaId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public override HttpMethod RequestMethod => HttpMethod.POST;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/message/send";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken }
      };
      return qs;
    }

    public override WeChatDictionary GetPostParameters()
    {
      var pp = new WeChatDictionary
      {
        { "touser", ToUser },
        { "toparty", ToParty },
        { "totag", ToTag },
        { "msgtype", MsgType },
        { "agentid", AgentId },
        { "video", new Dictionary<string, string> { { "media_id", MediaId }, { "title", Title }, { "description", Description } } },
        { "safe", Safe },
        { "enable_duplicate_check", EnableDuplicateCheck },
        { "duplicate_check_interval", DuplicateCheckInterval }
      };
      return pp;
    }
  }

  public class SendFileMessageRequest : WeChatRequest<MessageResponse>, IMessagePostEntity
  {
    public string ToUser { get; set; }
    public string ToParty { get; set; }
    public string ToTag { get; set; }
    public string MsgType
    { get { return "file"; } }
    public int AgentId { get; set; }
    public int Safe { get; set; } = 0;
    public int EnableDuplicateCheck { get; set; } = 0;
    public int DuplicateCheckInterval { get; set; } = 1800;

    public string MediaId { get; set; }

    public override HttpMethod RequestMethod => HttpMethod.POST;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/message/send";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken }
      };
      return qs;
    }

    public override WeChatDictionary GetPostParameters()
    {
      var pp = new WeChatDictionary
      {
        { "touser", ToUser },
        { "toparty", ToParty },
        { "totag", ToTag },
        { "msgtype", MsgType },
        { "agentid", AgentId },
        { "file", new Dictionary<string, string> { { "media_id", MediaId } } },
        { "safe", Safe },
        { "enable_duplicate_check", EnableDuplicateCheck },
        { "duplicate_check_interval", DuplicateCheckInterval }
      };
      return pp;
    }
  }

  public class SendTextCardMessageRequest : WeChatRequest<MessageResponse>, IMessagePostEntity
  {
    public string ToUser { get; set; }
    public string ToParty { get; set; }
    public string ToTag { get; set; }
    public string MsgType
    { get { return "textcard"; } }
    public int AgentId { get; set; }
    public int Safe { get; set; } = 0;
    public int EnableDuplicateCheck { get; set; } = 0;
    public int DuplicateCheckInterval { get; set; } = 1800;

    public int EnableIdTrans { get; set; } = 0;
    public string Title { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public string Btntxt { get; set; }

    public override HttpMethod RequestMethod => HttpMethod.POST;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/message/send";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken }
      };
      return qs;
    }

    public override WeChatDictionary GetPostParameters()
    {
      var pp = new WeChatDictionary
      {
        { "touser", ToUser },
        { "toparty", ToParty },
        { "totag", ToTag },
        { "msgtype", MsgType },
        { "agentid", AgentId },
        { "textcard", new Dictionary<string, string> { { "title", Title }, { "description", Description }, { "url", Url }, { "btntxt", Btntxt } } },
        { "safe", Safe },
        { "enable_id_trans", EnableIdTrans },
        { "enable_duplicate_check", EnableDuplicateCheck },
        { "duplicate_check_interval", DuplicateCheckInterval }
      };
      return pp;
    }
  }

  public class SendNewsMessageRequest : WeChatRequest<MessageResponse>, IMessagePostEntity
  {
    public string ToUser { get; set; }
    public string ToParty { get; set; }
    public string ToTag { get; set; }
    public string MsgType
    { get { return "news"; } }
    public int AgentId { get; set; }
    public int Safe { get; set; } = 0;
    public int EnableDuplicateCheck { get; set; } = 0;
    public int DuplicateCheckInterval { get; set; } = 1800;

    public int EnableIdTrans { get; set; } = 0;
    public ICollection<News> Articles { get; set; }

    public override HttpMethod RequestMethod => HttpMethod.POST;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/message/send";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken }
      };
      return qs;
    }

    public override WeChatDictionary GetPostParameters()
    {
      var pp = new WeChatDictionary
      {
        { "touser", ToUser },
        { "toparty", ToParty },
        { "totag", ToTag },
        { "msgtype", MsgType },
        { "agentid", AgentId },
        { "news", new Dictionary<string, object> { { "articles", Articles } } },
        { "enable_id_trans", EnableIdTrans },
        { "enable_duplicate_check", EnableDuplicateCheck },
        { "duplicate_check_interval", DuplicateCheckInterval }
      };
      return pp;
    }
  }

  public class SendMPNewsMessageRequest : WeChatRequest<MessageResponse>, IMessagePostEntity
  {
    public string ToUser { get; set; }
    public string ToParty { get; set; }
    public string ToTag { get; set; }
    public string MsgType
    { get { return "mpnews"; } }
    public int AgentId { get; set; }
    public int Safe { get; set; } = 0;
    public int EnableDuplicateCheck { get; set; } = 0;
    public int DuplicateCheckInterval { get; set; } = 1800;

    public int EnableIdTrans { get; set; } = 0;
    public ICollection<MpNews> Articles { get; set; }

    public override HttpMethod RequestMethod => HttpMethod.POST;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/message/send";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken }
      };
      return qs;
    }

    public override WeChatDictionary GetPostParameters()
    {
      var pp = new WeChatDictionary
      {
        { "touser", ToUser },
        { "toparty", ToParty },
        { "totag", ToTag },
        { "msgtype", MsgType },
        { "agentid", AgentId },
        { "mpnews", new Dictionary<string, object> { { "articles", Articles } } },
        { "safe", Safe },
        { "enable_id_trans", EnableIdTrans },
        { "enable_duplicate_check", EnableDuplicateCheck },
        { "duplicate_check_interval", DuplicateCheckInterval }
      };
      return pp;
    }
  }

  public class SendMiniProgramMessageRequest : WeChatRequest<MessageResponse>, IMessagePostEntity
  {
    public string ToUser { get; set; }
    public string ToParty { get; set; }
    public string ToTag { get; set; }
    public string MsgType
    { get { return "miniprogram_notice"; } }
    public int AgentId { get; set; }
    public int Safe { get; set; } = 0;
    public int EnableDuplicateCheck { get; set; } = 0;
    public int DuplicateCheckInterval { get; set; } = 1800;

    public int EnableIdTrans { get; set; } = 0;
    public MiniProgramNotice MiniProgramNotice { get; set; }

    public override HttpMethod RequestMethod => HttpMethod.POST;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/message/send";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken }
      };
      return qs;
    }

    public override WeChatDictionary GetPostParameters()
    {
      var pp = new WeChatDictionary
      {
        { "touser", ToUser },
        { "toparty", ToParty },
        { "totag", ToTag },
        { "msgtype", MsgType },
        { "miniprogram_notice", MiniProgramNotice },
        { "enable_id_trans", EnableIdTrans },
        { "enable_duplicate_check", EnableDuplicateCheck },
        { "duplicate_check_interval", DuplicateCheckInterval }
      };
      return pp;
    }
  }

  public class SendTaskCardMessageRequest : WeChatRequest<MessageResponse>, IMessagePostEntity
  {
    public string? ToUser { get; set; }
    public string? ToParty { get; set; }
    public string ToTag { get; set; }
    public string MsgType
    { get { return "taskcard"; } }
    public int AgentId { get; set; }
    public int Safe { get; set; } = 0;
    public int EnableDuplicateCheck { get; set; } = 0;
    public int DuplicateCheckInterval { get; set; } = 1800;

    public int EnableIdTrans { get; set; } = 0;
    public TaskCard TaskCard { get; set; }

    public override HttpMethod RequestMethod => HttpMethod.POST;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/message/send";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken }
      };
      return qs;
    }

    public override WeChatDictionary GetPostParameters()
    {
      var pp = new WeChatDictionary
      {
        { "touser", ToUser },
        { "toparty", ToParty },
        { "totag", ToTag },
        { "msgtype", MsgType },
        { "agentid", AgentId },
        { "taskcard", TaskCard },
        { "enable_id_trans", EnableIdTrans },
        { "enable_duplicate_check", EnableDuplicateCheck },
        { "duplicate_check_interval", DuplicateCheckInterval }
      };
      return pp;
    }
  }
}