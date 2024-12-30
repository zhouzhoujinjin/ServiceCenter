using PureCode.WeChat.Requests;

namespace PureCode.WeChat.Work.Requests
{
  public class GetWorkAccessTokenRequest : GetAccessTokenRequest
  {
    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var dict = new WeChatDictionary
      {
        { WeChatConsts.CorpId, context.AppId },
        { WeChatConsts.CorpSecret, context.Secret }
      };
      return dict;
    }

    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/gettoken";
  }
}