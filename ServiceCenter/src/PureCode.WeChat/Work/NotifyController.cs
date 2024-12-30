using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PureCode.WeChat.Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PureCode.WeChat.Work
{
  public class NotifyController : ControllerBase
  {
    private readonly WeChatContainer weChatContainer;

    public NotifyController(WeChatContainer weChatContainer)
    {
      this.weChatContainer = weChatContainer;
    }

    [HttpGet("/api/work/{appName}")]
    public IActionResult WorkCallback(
      [FromRoute(Name = "appName")] string appName,
      [FromQuery(Name = "msg_signature")] string signature,
      [FromQuery(Name = "timestamp")] string timestamp,
      [FromQuery(Name = "nonce")] string nonce,
      [FromQuery(Name = "echostr")] string echostr
      )
    {
      var context = weChatContainer.GetContextByName(appName);

      if (string.IsNullOrEmpty(context.NotifyToken) || string.IsNullOrEmpty(context.EncodingAesKey))
      {
        throw new Exception("未提供 NotifyToken 或 EncodingAesKey");
      }

      var helper = new MessageCrypto(context.NotifyToken, context.EncodingAesKey, context.AppId);
      var result = helper.VerifyURL(signature, timestamp, nonce, echostr, out string reply);
      return Content(reply);
    }

    [HttpPost("/api/work/{appName}")]
    public async Task<IActionResult> WorkCallback(
      [FromRoute(Name = "appName")] string appName,
      [FromQuery(Name = "msg_signature")] string signature,
      [FromQuery(Name = "timestamp")] string timestamp,
      [FromQuery(Name = "nonce")] string nonce
      )
    {
      HttpContext.Request.EnableBuffering();
      using var reader = new StreamReader(HttpContext.Request.Body);
      var str = await reader.ReadToEndAsync();

      var context = weChatContainer.GetContextByName(appName);

      if (string.IsNullOrEmpty(context.NotifyToken) || string.IsNullOrEmpty(context.EncodingAesKey))
      {
        throw new Exception("未提供 NotifyToken 或 EncodingAesKey");
      }

      var helper = new MessageCrypto(context.NotifyToken, context.EncodingAesKey, context.AppId);
      var ret = helper.DecryptMsg(signature, timestamp, nonce, str, out var echostr);
      if (ret != 0)
      {
        //异常
      }
      return Content(echostr);
    }
  }
}