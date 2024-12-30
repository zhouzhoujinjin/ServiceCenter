namespace PureCode.WeChat
{
  public abstract class WeChatRequest<TResponse> : IWeChatRequest<TResponse> where TResponse : WeChatResponse
  {
    public virtual HttpMethod RequestMethod => HttpMethod.GET;

    public virtual string CommandUrl => "";

    public virtual WeChatDictionary GetQueryString(WeChatContext context)
    {
      return null;
    }

    public virtual WeChatDictionary GetPostParameters()
    {
      return null;
    }
  }
}