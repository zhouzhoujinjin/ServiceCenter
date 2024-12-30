using System.Xml.Serialization;

namespace PureCode.WeChat.Work
{
  public abstract class WeChatWorkNotify
  {
    /// <summary>
    /// 企业微信的CorpID
    /// </summary>

    [XmlElement("ToUserName")]
    public string ToUserName { get; set; }

    /// <summary>
    /// 接收的应用id，可在应用的设置页面获取。仅应用相关的回调会带该字段。
    /// </summary>
    [XmlElement("AgentID")]
    public string AgentId { get; internal set; }

    /// <summary>
    /// 消息结构体加密后的字符串
    /// </summary>
    [XmlElement("Encrypt")]
    public string Encrypt { get; set; }

    /// <summary>
    /// 处理 _$n / _$n_$m
    /// </summary>
    internal virtual void Execute()
    { }
  }
}