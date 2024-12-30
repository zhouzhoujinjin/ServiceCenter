using Microsoft.Extensions.Logging;
using PureCode.Utils;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PureCode.WeChat
{
  public class WeChatClient : IWeChatClient
  {
    public const string Prefix = nameof(WeChatClient) + ".";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WeChatClient> logger;

    public WeChatClient(IHttpClientFactory httpClientFactory, ILogger<WeChatClient> logger)
    {
      _httpClientFactory = httpClientFactory;
      this.logger = logger;
    }

    public async Task<T> ExecuteAsync<T>(IWeChatRequest<T> request, WeChatContext context) where T : WeChatResponse, new()
    {
      if (context == null)
      {
        throw new ArgumentNullException(nameof(context));
      }

      if (string.IsNullOrEmpty(context.AppId))
      {
#pragma warning disable CA2208 // 正确实例化参数异常
        throw new ArgumentNullException(nameof(context.AppId));
#pragma warning restore CA2208 // 正确实例化参数异常
      }

      var client = _httpClientFactory.CreateClient(nameof(WeChatClient));

      object body = null;
      if (request.RequestMethod == HttpMethod.GET)
      {
        body = await client.GetAsync(request.CommandUrl, request.GetQueryString(context));
      }
      else if (request.RequestMethod == HttpMethod.POST)
      {
        var postData = request.GetPostParameters();
        body = await client.PostAsync(request.CommandUrl, request.GetQueryString(context), postData);
      }
      try
      {
        if (body is string)
        {
          var response = JsonSerializer.Deserialize<T>(body as string);
          response.AppId = context.AppId;
          return response;
        }
        else
        {
          var response = new T
          {
            AppId = context.AppId,
            Addon = body
          };

          return response;
        }
      }
      catch (Exception e)
      {
        return new T { ErrorCode = 500, ErrorMessage = e.Message };
      }
    }

    public Task<WeChatDictionary> ExecuteAsync(IWeChatSdkRequest request, WeChatContext context)
    {
      throw new NotImplementedException();
    }

    public Task<T> PageExecuteAsync<T>(IWeChatRequest<T> request, WeChatContext context) where T : WeChatResponse, new()
    {
      throw new NotImplementedException();
    }
  }
}