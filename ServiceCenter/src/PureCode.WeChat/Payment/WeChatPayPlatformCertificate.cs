using System;
using System.Security.Cryptography.X509Certificates;

namespace PureCode.WeChat.Payment
{
  public class WeChatPayPlatformCertificate
  {
    /// <summary>
    /// 商户号
    /// </summary>
    public string MchId { get; set; }

    /// <summary>
    /// 序列号
    /// </summary>
    public string SerialNo { get; set; }

    /// <summary>
    /// 生效时间
    /// </summary>
    public DateTime EffectiveTime { get; set; }

    /// <summary>
    /// 失效时间
    /// </summary>
    public DateTime ExpireTime { get; set; }

    /// <summary>
    /// 证书
    /// </summary>
    public X509Certificate2 Certificate;
  }
}