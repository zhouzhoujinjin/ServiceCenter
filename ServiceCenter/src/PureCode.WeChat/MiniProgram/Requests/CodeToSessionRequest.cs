using PureCode.WeChat.MiniProgram.Responses;

namespace PureCode.WeChat.MiniProgram.Requests
{
  public class CodeToSessionRequest : WeChatRequest<CodeToSessionResponse>
  {
    public string? Code { get; set; }

    public override string CommandUrl => "https://api.weixin.qq.com/sns/jscode2session";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChatConsts.AppId, context.AppId },
        { WeChatConsts.Secret, context.Secret },
        { "js_code", Code },
        { WeChatConsts.GrantType, "authorization_code" }
      };
      return qs;
    }
  }
}