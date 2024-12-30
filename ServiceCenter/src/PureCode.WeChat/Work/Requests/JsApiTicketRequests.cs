using PureCode.WeChat.Work.Responses;

namespace PureCode.WeChat.Work.Requests
{
  public class JsApiTicketRequest : WeChatRequest<JsApiTicketReponse>
  {
    public override HttpMethod RequestMethod => HttpMethod.GET;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/get_jsapi_ticket";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var dict = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken },
      };
      return dict;
    }
  }
}