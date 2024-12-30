using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PureCode.WeChat
{
  public interface IWeChatClient
  {
    /// <summary>
    /// 执行 WeChat API请求。
    /// </summary>
    /// <param name="request">具体的WeChatPay API请求</param>
    /// <param name="options">配置选项</param>
    /// <returns>领域对象</returns>
    Task<T> ExecuteAsync<T>(IWeChatRequest<T> request, WeChatContext options) where T : WeChatResponse, new();

    /// <summary>
    /// 执行 WeChatPay 服务器端 API 请求。
    /// </summary>
    /// <param name="request">具体的WeChat 服务器端 API 请求</param>
    /// <param name="options">配置选项</param>
    /// <returns>领域对象</returns>
    Task<T> PageExecuteAsync<T>(IWeChatRequest<T> request, WeChatContext options) where T : WeChatResponse, new();

    /// <summary>
    /// 执行模拟 WeChat 客户端 Sdk 请求。
    /// </summary>
    /// <param name="request">具体的WeChat 客户端 Sdk 请求</param>
    /// <param name="options">配置选项</param>
    Task<WeChatDictionary> ExecuteAsync(IWeChatSdkRequest request, WeChatContext options);
  }

  public interface IWeChatRequest<T>
  {
    /// <summary>
    /// 请求方式，可选值 "GET"、"POST"
    /// </summary>
    HttpMethod RequestMethod { get; }

    /// <summary>
    /// 获取API接口链接
    /// </summary>
    /// <returns>API接口链接</returns>
    string CommandUrl { get; }

    WeChatDictionary GetQueryString(WeChatContext context);

    /// <summary>
    /// 获取所有的Key-Value形式的文本请求参数字典。其中：
    /// Key: 请求参数名
    /// Value: 请求参数文本值
    /// </summary>
    /// <returns>文本请求参数字典</returns>
    WeChatDictionary GetPostParameters();
  }

  public interface IWeChatSdkRequest
  {
    /// <summary>
    /// 获取所有的Key-Value形式的文本请求参数字典。其中：
    /// Key: 请求参数名
    /// Value: 请求参数文本值
    /// </summary>
    /// <returns>文本请求参数字典</returns>
    IDictionary<string, string> GetParameters();

    /// <summary>
    /// 请求参数处理器
    /// </summary>
    /// <param name="options"></param>
    /// <param name="sortedTxtParams"></param>
    void PrimaryHandler(WeChatContext options, WeChatDictionary sortedTxtParams);
  }

  public interface IWeChatNotifyClient
  {
  }

  public interface IWeChatResolver
  {
    WeChatContainer Container { get; }
    WeChatContext Context { get; }
  }

  public interface IContextStore
  {
    public Task SaveContextAsync(string appId, string accessToken, DateTime dateTime);

    public Task<WeChatContext> GetContextAsync(string appId);

    public WeChatContext GetContext(string appId);
  }

  public interface IJsTicketStore
  {
    public Task SaveTicketAsync(string appId, string ticket, DateTime dateTime);

    public Task<string> GetTicketAsync(string appId);

    public string GetTicket(string appId);
  }

  public interface IWeChatUserStore
  {
    public Task SaveUserAsync(string appId, WeChatUser user);

    public Task<WeChatUser> GetUserAsync(string appId, string openId);
  }
}