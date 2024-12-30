using Microsoft.AspNetCore.Http;
using PureCode.WeChat.Work.Responses;

namespace PureCode.WeChat.Work.Requests
{
  //上传临时素材
  public class UploadMediaRequest : WeChatRequest<UploadMediaResponse>
  {
    public string Type { get; set; }
    public IFormFile File { get; set; }

    public override HttpMethod RequestMethod => HttpMethod.POST;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/media/upload";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken },
        { "type", Type }
      };
      return qs;
    }

    public override WeChatDictionary GetPostParameters()
    {
      var pp = new WeChatDictionary
      {
        { "media", File }
      };
      return pp;
    }
  }

  //获取临时素材
  public class DownloadMediaRequest : WeChatRequest<WeChatResponse>
  {
    public string MediaId { get; set; }

    public override HttpMethod RequestMethod => HttpMethod.GET;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/media/get";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken },
        { "media_id", MediaId }
      };
      return qs;
    }
  }
}