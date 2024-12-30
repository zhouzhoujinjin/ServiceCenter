using PureCode.WeChat.MiniProgram.Responses;

namespace PureCode.WeChat.MiniProgram.Requests
{
  internal class GetPaidUnionIdRequest : WeChatRequest<CodeToSessionResponse>
  {
    public string? AccessToken { get; set; }
    public string? OpenId { get; set; }

    public string? OutTradeNo { get; set; }
    public string? TransactionId { get; set; }
    public string? MerchantId { get; set; }

    public string GetRequestUrl() => "https://api.weixin.qq.com/wxa/getpaidunionid";
  }
}