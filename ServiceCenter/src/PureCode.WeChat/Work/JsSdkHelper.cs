using PureCode.Security;
using System;
using System.Text;

namespace PureCode.WeChat.Work
{
  public class JsSdkHelper
  {
    /// <summary>
    /// 获取随机字符串
    /// </summary>
    /// <returns></returns>
    public static string GetNoncestr()
    {
      var encypStr = Guid.NewGuid().ToString();

      return MD5.Compute(encypStr).ToUpper();
    }

    /// <summary>
    /// 获取时间戳
    /// <remarks>
    /// </remarks>
    /// </summary>
    /// <returns></returns>
    public static long GetTimestamp()
    {
      TimeSpan ts = DateTimeOffset.Now - new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);
      return Convert.ToInt64(ts.TotalSeconds);
    }

    /// <summary>
    /// 获取JS-SDK权限验证的签名Signature
    /// </summary>
    /// <param name="jsapi_ticket">jsapi_ticket</param>
    /// <param name="noncestr">随机字符串(必须与wx.config中的nonceStr相同)</param>
    /// <param name="timestamp">时间戳(必须与wx.config中的timestamp相同)</param>
    /// <param name="url">当前网页的URL，不包含#及其后面部分(必须是调用JS接口页面的完整URL)</param>
    /// <returns></returns>
    public static string GetSignature(string jsapi_ticket, string noncestr, long timestamp, string url)
    {
      var sb = new StringBuilder();
      sb.Append("jsapi_ticket=").Append(jsapi_ticket).Append('&')
       .Append("noncestr=").Append(noncestr).Append('&')
       .Append("timestamp=").Append(timestamp).Append('&')
       .Append("url=").Append(url.Contains("#") ? url.Substring(0, url.IndexOf("#")) : url);

      return SHA1.Compute(sb.ToString()).ToLower();
    }
  }
}