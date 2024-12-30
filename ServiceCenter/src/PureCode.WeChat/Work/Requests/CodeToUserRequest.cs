using PureCode.WeChat.Work.Responses;

namespace PureCode.WeChat.Work.Requests
{
  public class CodeToUserRequest : WeChatRequest<CodeToUserResponse>
  {
    public string Code { get; set; }

    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken },
        { "code", Code }
      };
      return qs;
    }
  }
}