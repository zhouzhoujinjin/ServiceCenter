using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PureCode.WeChat
{
  public enum AccountType
  {
    OfficalAccount,
    MiniProgram,
    Work
  }

  public class TemplateMessageInfo
  {
    public string TemplateId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<string> Keys { get; set; }
  }

  public class AccountInfo
  {
    public string Latest { get; set; }

    /// <summary>
    /// 微信应用号(公众平台AppId/开放平台AppId/小程序AppId/企业微信CorpId)
    /// </summary>
    public string AppId { get; set; }

    /// <summary>
    /// 企业微信应用需要的应用 AgentId
    /// </summary>
    public int AgentId { get; set; }

    /// <summary>
    /// 企业微信应用需要的应用 HomeUrl
    /// </summary>
    public string? HomeUrl { get; set; }

    /// <summary>
    /// 微信应用号的自定义名称
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// 微信应用号的类型
    /// </summary>
    public string AppType { get; set; }

    /// <summary>
    /// 微信应用密钥(企业微信Secret，目前仅"企业红包API"使用)
    /// </summary>
    public string Secret { get; set; }

    public string? NotifyToken { get; set; }
    public string? EncodingAesKey { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AccountType AccountType { get; set; }

    public Dictionary<string, TemplateMessageInfo> TemplateMessageInfos { get; set; }

    public AccountInfo()
    {
      TemplateMessageInfos = new Dictionary<string, TemplateMessageInfo>();
    }
  }

  public class WeChatOptions
  {
    public ICollection<AccountInfo> Items { get; set; }
    public string DefaultCacheNamespace { get; set; }
    public string AppIdHeaderKey { get; set; }
    public WeChatOptions Value { get; set; }

    public WeChatOptions()
    {
      Items = new List<AccountInfo>();
    }
  }
}