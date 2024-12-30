using PureCode.WeChat.Responses;

namespace PureCode.WeChat.Requests
{
  public class GetAccessTokenRequest : WeChatRequest<AccessTokenResponse>
  {
    public override HttpMethod RequestMethod => HttpMethod.GET;

    public string AppId { get; set; }

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var dict = new WeChatDictionary
      {
        { WeChatConsts.AppId, context.AppId },
        { WeChatConsts.Secret, context.Secret },
        { WeChatConsts.GrantType, "client_credential" }
      };
      return dict;
    }

    public override string CommandUrl => "https://api.weixin.qq.com/cgi-bin/token";

    public void PrimaryHandler(WeChatContext context, WeChatDictionary sortedTxtParams)
    {
    }
  }
}