using Microsoft.AspNetCore.Http;
using PureCode.WeChat.Responses;
using PureCode.WeChat.Work.Requests;
using PureCode.WeChat.Work.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace PureCode.WeChat.Work
{
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2208:正确实例化参数异常", Justification = "<挂起>")]
  public class WeChatWorkApi
  {
    private readonly IWeChatClient client;
    private readonly IWeChatResolver wechatResolver;
    private readonly WeChatContainer container;
    private readonly IJsTicketStore jsTicketStore;

    public WeChatWorkApi(IWeChatClient client, IJsTicketStore jsTicketStore, IWeChatResolver wechatResolver, WeChatContainer container)
    {
      this.client = client;
      this.wechatResolver = wechatResolver;
      this.container = container;
      this.jsTicketStore = jsTicketStore;
    }

    private async Task CheckTokenAsync(string appName)
    {
      var at = container.GetContextByName(appName);
      if (string.IsNullOrEmpty(at.AccessToken) || at.AccessTokenExpiredTime < DateTime.Now)
      {
        await GetAccessTokenAsync(at.AppName);
      }
    }

    private string GetWorkAppId(string appName)
    {
      var context = container.GetContextByName(appName);
      return $"{context.AppId}.{context.AgentId}";
    }

    public async Task<AccessTokenResponse> GetAccessTokenAsync(string appName)
    {
      var req = new GetWorkAccessTokenRequest();
      var res = await client.ExecuteAsync(req, container.GetContextByName(appName));
      await wechatResolver.Container.UpdateContextAccessToken(GetWorkAppId(appName), res.AccessToken, res.ExpiresIn);
      return res;
    }

    public string GenerateOAuthUrl(string appName, string authUrl, string redirectUrl)
    {
      var finalRedirectUrl = $"{authUrl}?redirectUrl={HttpUtility.UrlEncode(redirectUrl)}";
      var context = container.GetContextByName(appName);
      return $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={context.AppId}&redirect_uri={HttpUtility.UrlEncode(finalRedirectUrl)}&response_type=code&scope=snsapi_base&state=#wechat_redirect";
    }

    public async Task<(string, string)> GetJsApiTicketAsync(string appName)
    {
      await CheckTokenAsync(appName);
      var context = container.GetContextByName(appName);

      var key = $"{context.AppId}.{context.AgentId}";
      var ticket = await jsTicketStore.GetTicketAsync(key);
      if (ticket == null)
      {
        var req = new JsApiTicketRequest();
        var res = await client.ExecuteAsync(req, context);
        ticket = res.Ticket;
        await jsTicketStore.SaveTicketAsync(key, ticket, DateTime.Now.AddSeconds(res.ExpiresTime - 20));
      }
      return (ticket, context.AppId);
    }

    public async Task<CodeToUserResponse> LoginAsync(string appName, string code)
    {
      await CheckTokenAsync(appName);
      var req = new CodeToUserRequest
      {
        Code = code
      };
      var context = container.GetContextByName(appName);
      var res = await client.ExecuteAsync(req, context);

      if (string.IsNullOrEmpty(res.UserId) && !string.IsNullOrEmpty(res.OpenId))
      {
        var convertReq = new ConvertUserIdRequest
        {
          OpenId = res.OpenId
        };
        var info = await client.ExecuteAsync(convertReq, context);
        res.UserId = info.UserId;
      }

      await wechatResolver.Container.StoreUserAsync(context.AppId, new WeChatUser
      {
        OpenId = res.OpenId,
        UnionId = res.UserId
      });
      return res;
    }

    #region 部门管理

    public async Task<ListDepartmentsResponse> ListDepartmentAsync(int? id = null)
    {
      var req = new ListDepartmentsRequest
      {
        Id = id
      };
      await CheckTokenAsync("contact");
      var context = container.GetContextByName("contact");
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    public async Task<WeChatResponse> DeleteDepartmentsAsync(int id)
    {
      var req = new DeleteDepartmentsRequest
      {
        Id = id
      };
      await CheckTokenAsync("contact");
      var context = container.GetContextByName("contact");
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    public async Task<CreateDepartmentResponse> CreateDepartmentAsync(string name, int? parentId = null, int? order = null, string? nameEn = null)
    {
      var req = new CreateDepartmentRequest
      {
        DepartmentName = name,
        DepartmentNameEn = nameEn,
        ParentId = parentId,
        Order = order
      };
      await CheckTokenAsync("contact");
      var context = container.GetContextByName("contact");
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    public async Task<WeChatResponse> UpdateDepartmentAsync(int id, string name, int? parentId = null, int? order = null, string? nameEn = null)
    {
      var req = new UpdateDepartmentRequest
      {
        DepartmentId = id,
        DepartmentName = name,
        DepartmentNameEn = nameEn,
        ParentId = parentId,
        Order = order
      };
      await CheckTokenAsync("contact");
      var context = container.GetContextByName("contact");
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    #endregion 部门管理

    #region 成员管理

    /// <summary>
    /// 创建成员
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="name"></param>
    /// <param name="mobile"></param>
    /// <param name="department"></param>
    /// <returns></returns>
    public async Task<WeChatResponse> CreateUserAsync(string userId, string name, string mobile, ICollection<int>? department = null)
    {
      var req = new CreateUserRequest
      {
        UserId = userId,
        UserName = name,
        Mobile = mobile,
        Department = department
      };
      await CheckTokenAsync("contact");
      var context = container.GetContextByName("contact");
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    /// <summary>
    /// 获得成员信息
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<GetUserResponse> GetUserAsync(string userId)
    {
      var req = new GetUserRequest
      {
        UserId = userId
      };
      await CheckTokenAsync("contact");
      var context = container.GetContextByName("contact");
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    /// <summary>
    /// 修改成员信息，这里只是局部信息如果需要修改其他详细信息，可以适当的扩充接口
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="name"></param>
    /// <param name="alias"></param>
    /// <param name="mobile"></param>
    /// <param name="position"></param>
    /// <param name="department"></param>
    /// <returns></returns>
    public async Task<WeChatResponse> UpdateUserAsync(string userId, string name, string alias, string mobile, string position, ICollection<int>? department = null)
    {
      var req = new UpdateUserRequest
      {
        UserId = userId,
        UserName = name,
        UserAlias = alias,
        Mobile = mobile,
        Department = department,
        Position = position
      };
      await CheckTokenAsync("contact");
      var context = container.GetContextByName("contact");
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    /// <summary>
    /// 删除成员
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<WeChatResponse> DeleteUserAsync(string userId)
    {
      var req = new DeleteUserRequest
      {
        UserId = userId
      };
      await CheckTokenAsync("contact");
      var context = container.GetContextByName("contact");
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    /// <summary>
    /// 批量删除成员
    /// </summary>
    /// <param name="userIds"></param>
    /// <returns></returns>
    public async Task<WeChatResponse> BatchDeleteUserAsync(ICollection<string> userIds)
    {
      var req = new BatchDeleteUserRequest
      {
        UserIdList = userIds
      };
      await CheckTokenAsync("contact");
      var context = container.GetContextByName("contact");
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    /// <summary>
    /// 获得部门人员
    /// </summary>
    /// <param name="departmentId"></param>
    /// <param name="fetchChild">是否递归获取子部门下面的成员：1-递归获取，0-只获取本部门</param>
    /// <returns></returns>
    public async Task<UserListResponse> GetDepartmentUsersAsync(int departmentId, int? fetchChild = 1)
    {
      var req = new GetDepartmentUsersRequest
      {
        DepartmentId = departmentId,
        FetchChild = fetchChild
      };
      await CheckTokenAsync("contact");
      var context = container.GetContextByName("contact");
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    /// <summary>
    /// 获得部门人员详细信息
    /// </summary>
    /// <param name="departmentId"></param>
    /// <param name="fetchChild"></param>
    /// <returns></returns>
    public async Task<UserDetailListResponse> GetDepartmentUsersDetailAsync(int departmentId, int? fetchChild = 1)
    {
      var req = new GetDepartmentUsersDetailRequest
      {
        DepartmentId = departmentId,
        FetchChild = fetchChild
      };
      await CheckTokenAsync("contact");
      var context = container.GetContextByName("contact");
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    /// <summary>
    /// 根据userId获得openId
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ConvertOpenIdResponse> ConvertOpenIdAsync(string userId)
    {
      var req = new ConvertOpenIdRequest
      {
        UserId = userId
      };
      await CheckTokenAsync("contact");
      var context = container.GetContextByName("contact");
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    /// <summary>
    /// 根据openId获得userId
    /// </summary>
    /// <param name="openId"></param>
    /// <returns></returns>
    public async Task<ConvertUserIdResponse> ConvertUserIdAsync(string openId)
    {
      var req = new ConvertUserIdRequest
      {
        OpenId = openId
      };
      await CheckTokenAsync("contact");
      var context = container.GetContextByName("contact");
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    /// <summary>
    /// 二次校验
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<WeChatResponse> AuthsuccAsync(string userId)
    {
      var req = new AuthsuccRequest
      {
        UserId = userId
      };
      await CheckTokenAsync("contact");
      var context = container.GetContextByName("contact");
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    /// <summary>
    /// 邀请成员
    /// </summary>
    /// <param name="userIds"></param>
    /// <param name="departmentIds"></param>
    /// <param name="tags"></param>
    /// <returns></returns>
    public async Task<InviteResponse> InviteUserOrDepartmentAsync(ICollection<string>? userIds = null, ICollection<int>? departmentIds = null, ICollection<int> tags = null)
    {
      if (userIds == null && departmentIds == null && tags == null)
      {
        throw new NotImplementedException();
      }
      var req = new InviteUserOrDepartmentRequest
      {
        UserIds = userIds,
        DepartmentIds = departmentIds,
        Tags = tags
      };
      await CheckTokenAsync("contact");
      var context = container.GetContextByName("contact");
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    /// <summary>
    /// 获得加入企业二维码
    /// </summary>
    /// <param name="sizeType">1: 171 x 171; 2: 399 x 399; 3: 741 x 741; 4: 2052 x 2052</param>
    /// <returns></returns>
    public async Task<GetJoinQrcodeResponse> GetJoinQrcodeAsync(int? sizeType = 1)
    {
      var req = new GetJoinQrcodeRequest
      {
        SizeType = sizeType
      };
      await CheckTokenAsync("contact");
      var context = container.GetContextByName("contact");
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    /// <summary>
    /// 获取企业活跃成员数
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public async Task<GetActiveStatResponse> GetActiveStatAsync(string date)
    {
      var req = new GetActiveStatRequest
      {
        Date = date
      };
      await CheckTokenAsync("contact");
      var context = container.GetContextByName("contact");
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    #endregion 成员管理

    #region 消息推送

    /// <summary>
    /// 文本消息
    /// </summary>
    /// <param name="appName">应用名称(配置文件中的AppName)</param>
    /// <param name="agentId">应用AgentId</param>
    /// <param name="toUser">接收人员的UserId，多个用 | 隔开, 指定为”@all”，则向该企业应用的全部成员发送</param>
    /// <param name="toParty">接收部门Id，多个用 | 隔开</param>
    /// <param name="toTag">接收标签Id，多个用 | 隔开</param>
    /// <param name="content">发送内容</param>
    /// <returns></returns>
    public async Task<MessageResponse> SendTextMessageAsync(string appName, string toUser = "", string toParty = "", string toTag = "", string content = "")
    {
      if (string.IsNullOrEmpty(toUser) && string.IsNullOrEmpty(toParty) && string.IsNullOrEmpty(toTag))
      {
        throw new NotImplementedException();
      }
      await CheckTokenAsync(appName);
      var context = container.GetContextByName(appName);
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }

      var req = new SendTextMessageRequest
      {
        ToUser = toUser,
        ToParty = toParty,
        ToTag = toTag,
        AgentId = context.AgentId,
        Content = content
      };

      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    /// <summary>
    /// 图片消息
    /// </summary>
    /// <param name="appName">应用名称(配置文件中的AppName)</param>
    /// <param name="agentId">应用AgentId</param>
    /// <param name="toUser">接收人员的UserId，多个用 | 隔开, 指定为”@all”，则向该企业应用的全部成员发送</param>
    /// <param name="toParty">接收部门Id，多个用 | 隔开</param>
    /// <param name="toTag">接收标签Id，多个用 | 隔开</param>
    /// <param name="mediaId">图片媒体文件id，可以调用上传临时素材接口获取</param>
    /// <returns></returns>
    public async Task<MessageResponse> SendImageMessageAsync(string appName, string toUser = "", string toParty = "", string toTag = "", string mediaId = "")
    {
      if (string.IsNullOrEmpty(toUser) && string.IsNullOrEmpty(toParty) && string.IsNullOrEmpty(toTag))
      {
        throw new NotImplementedException();
      }
      await CheckTokenAsync(appName);
      var context = container.GetContextByName(appName);
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var req = new SendImageMessageRequest
      {
        ToUser = toUser,
        ToParty = toParty,
        ToTag = toTag,
        AgentId = context.AgentId,
        MediaId = mediaId
      };
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    /// <summary>
    /// 语音消息
    /// </summary>
    /// <param name="appName">应用名称(配置文件中的AppName)</param>
    /// <param name="agentId">应用AgentId</param>
    /// <param name="toUser">接收人员的UserId，多个用 | 隔开, 指定为”@all”，则向该企业应用的全部成员发送</param>
    /// <param name="toParty">接收部门Id，多个用 | 隔开</param>
    /// <param name="toTag">接收标签Id，多个用 | 隔开</param>
    /// <param name="mediaId">图片媒体文件id，可以调用上传临时素材接口获取</param>
    /// <returns></returns>
    public async Task<MessageResponse> SendVoiceMessageAsync(string appName, string toUser = "", string toParty = "", string toTag = "", string mediaId = "")
    {
      if (string.IsNullOrEmpty(toUser) && string.IsNullOrEmpty(toParty) && string.IsNullOrEmpty(toTag))
      {
        throw new NotImplementedException();
      }
      await CheckTokenAsync(appName);
      var context = container.GetContextByName(appName);
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var req = new SendVoiceMessageRequest
      {
        ToUser = toUser,
        ToParty = toParty,
        ToTag = toTag,
        AgentId = context.AgentId,
        MediaId = mediaId
      };
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    /// <summary>
    /// 视频消息
    /// </summary>
    /// <param name="appName">应用名称(配置文件中的AppName)</param>
    /// <param name="agentId">应用AgentId</param>
    /// <param name="toUser">接收人员的UserId，多个用 | 隔开, 指定为”@all”，则向该企业应用的全部成员发送</param>
    /// <param name="toParty">接收部门Id，多个用 | 隔开</param>
    /// <param name="toTag">接收标签Id，多个用 | 隔开</param>
    /// <param name="mediaId">图片媒体文件id，可以调用上传临时素材接口获取</param>
    /// <returns></returns>
    public async Task<MessageResponse> SendVideoMessageAsync(string appName, string toUser = "", string toParty = "", string toTag = "",
      string mediaId = "", string title = "", string description = "")
    {
      if (string.IsNullOrEmpty(toUser) && string.IsNullOrEmpty(toParty) && string.IsNullOrEmpty(toTag))
      {
        throw new NotImplementedException();
      }
      await CheckTokenAsync(appName);
      var context = container.GetContextByName(appName);
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var req = new SendVideoMessageRequest
      {
        ToUser = toUser,
        ToParty = toParty,
        ToTag = toTag,
        AgentId = context.AgentId,
        MediaId = mediaId,
        Title = title,
        Description = description
      };
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    /// <summary>
    /// 文件消息
    /// </summary>
    /// <param name="appName">应用名称(配置文件中的AppName)</param>
    /// <param name="agentId">应用AgentId</param>
    /// <param name="toUser">接收人员的UserId，多个用 | 隔开, 指定为”@all”，则向该企业应用的全部成员发送</param>
    /// <param name="toParty">接收部门Id，多个用 | 隔开</param>
    /// <param name="toTag">接收标签Id，多个用 | 隔开</param>
    /// <param name="mediaId">图片媒体文件id，可以调用上传临时素材接口获取</param>
    /// <returns></returns>
    public async Task<MessageResponse> SendFileMessageAsync(string appName, string toUser = "", string toParty = "", string toTag = "", string mediaId = "")
    {
      if (string.IsNullOrEmpty(toUser) && string.IsNullOrEmpty(toParty) && string.IsNullOrEmpty(toTag))
      {
        throw new NotImplementedException();
      }
      await CheckTokenAsync(appName);
      var context = container.GetContextByName(appName);
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var req = new SendFileMessageRequest
      {
        ToUser = toUser,
        ToParty = toParty,
        ToTag = toTag,
        AgentId = context.AgentId,
        MediaId = mediaId
      };
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    /// <summary>
    /// 文本卡片消息
    /// </summary>
    /// <param name="appName">应用名称(配置文件中的AppName)</param>
    /// <param name="agentId">应用AgentId</param>
    /// <param name="toUser">接收人员的UserId，多个用 | 隔开, 指定为”@all”，则向该企业应用的全部成员发送</param>
    /// <param name="toParty">接收部门Id，多个用 | 隔开</param>
    /// <param name="toTag">接收标签Id，多个用 | 隔开</param>
    /// <returns></returns>
    public async Task<MessageResponse> SendTextCardMessageAsync(string appName, string toUser = "", string toParty = "", string toTag = "",
      string title = "", string description = "", string url = "", string btntxt = "")
    {
      if (string.IsNullOrEmpty(toUser) && string.IsNullOrEmpty(toParty) && string.IsNullOrEmpty(toTag))
      {
        throw new NotImplementedException();
      }
      await CheckTokenAsync(appName);
      var context = container.GetContextByName(appName);
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var req = new SendTextCardMessageRequest
      {
        ToUser = toUser,
        ToParty = toParty,
        ToTag = toTag,
        AgentId = context.AgentId,
        Title = title,
        Description = description,
        Url = url,
        Btntxt = btntxt
      };
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    /// <summary>
    /// 图文消息
    /// </summary>
    /// <param name="appName">应用名称(配置文件中的AppName)</param>
    /// <param name="agentId">应用AgentId</param>
    /// <param name="toUser">接收人员的UserId，多个用 | 隔开, 指定为”@all”，则向该企业应用的全部成员发送</param>
    /// <param name="toParty">接收部门Id，多个用 | 隔开</param>
    /// <param name="toTag">接收标签Id，多个用 | 隔开</param>
    /// <param name="articles"></param>
    /// <returns></returns>
    public async Task<MessageResponse> SendNewsMessageAsync(string appName, string toUser = "", string toParty = "", string toTag = "", ICollection<News> articles = null)
    {
      if (string.IsNullOrEmpty(toUser) && string.IsNullOrEmpty(toParty) && string.IsNullOrEmpty(toTag) && articles == null)
      {
        throw new NotImplementedException();
      }
      await CheckTokenAsync(appName);
      var context = container.GetContextByName(appName);
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var req = new SendNewsMessageRequest
      {
        ToUser = toUser,
        ToParty = toParty,
        ToTag = toTag,
        AgentId = context.AgentId,
        Articles = articles
      };
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    /// <summary>
    /// 图文消息MP
    /// 跟普通的图文消息一致，唯一的差异是图文内容存储在企业微信。
    /// </summary>
    /// <param name="appName">应用名称(配置文件中的AppName)</param>
    /// <param name="agentId">应用AgentId</param>
    /// <param name="toUser">接收人员的UserId，多个用 | 隔开, 指定为”@all”，则向该企业应用的全部成员发送</param>
    /// <param name="toParty">接收部门Id，多个用 | 隔开</param>
    /// <param name="toTag">接收标签Id，多个用 | 隔开</param>
    /// <param name="articles"></param>
    /// <returns></returns>
    public async Task<MessageResponse> SendMPNewsMessageAsync(string appName, string toUser = "", string toParty = "", string toTag = "", ICollection<MpNews> articles = null)
    {
      if (string.IsNullOrEmpty(toUser) && string.IsNullOrEmpty(toParty) && string.IsNullOrEmpty(toTag) && articles == null)
      {
        throw new NotImplementedException();
      }
      await CheckTokenAsync(appName);
      var context = container.GetContextByName(appName);
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var req = new SendMPNewsMessageRequest
      {
        ToUser = toUser,
        ToParty = toParty,
        ToTag = toTag,
        AgentId = context.AgentId,
        Articles = articles
      };
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    /// <summary>
    /// 小程序通知消息
    /// 小程序应用仅支持发送小程序通知消息，暂不支持文本、图片、语音、视频、图文等其他类型的消息。不支持 @all全员发送
    /// </summary>
    /// <param name="appName">应用名称(配置文件中的AppName)</param>
    /// <param name="toUser">接收人员的UserId，多个用 | 隔开, 指定为”@all”，则向该企业应用的全部成员发送</param>
    /// <param name="toParty">接收部门Id，多个用 | 隔开</param>
    /// <param name="toTag">接收标签Id，多个用 | 隔开</param>
    /// <param name="miniProgramNotice"></param>
    /// <returns></returns>
    public async Task<MessageResponse> SendMiniProgramMessageAsync(string appName, string toUser = "", string toParty = "", string toTag = "", MiniProgramNotice miniProgramNotice = null)
    {
      if (string.IsNullOrEmpty(toUser) && string.IsNullOrEmpty(toParty) && string.IsNullOrEmpty(toTag) && miniProgramNotice == null)
      {
        throw new NotImplementedException();
      }
      await CheckTokenAsync(appName);
      var context = container.GetContextByName(appName);
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var req = new SendMiniProgramMessageRequest
      {
        ToUser = toUser,
        ToParty = toParty,
        ToTag = toTag,
        MiniProgramNotice = miniProgramNotice
      };
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    /// <summary>
    /// 任务卡片消息
    /// </summary>
    /// <param name="appName">应用名称(配置文件中的AppName)</param>
    /// <param name="agentId">应用AgentId</param>
    /// <param name="toUser">接收人员的UserId，多个用 | 隔开, 指定为”@all”，则向该企业应用的全部成员发送</param>
    /// <param name="toParty">接收部门Id，多个用 | 隔开</param>
    /// <param name="toTag">接收标签Id，多个用 | 隔开</param>
    /// <param name="taskCard"></param>
    /// <returns></returns>
    public async Task<MessageResponse> SendTaskCardMessageAsync(string appName, string toUser = "", string toParty = "", string toTag = "", TaskCard taskCard = null)
    {
      if (string.IsNullOrEmpty(toUser) && string.IsNullOrEmpty(toParty) && string.IsNullOrEmpty(toTag) && taskCard == null)
      {
        throw new NotImplementedException();
      }
      if (taskCard.Btn != null && taskCard.Btn.Count > 2)
      {
        throw new NotImplementedException();
      }
      await CheckTokenAsync(appName);
      var context = container.GetContextByName(appName);
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var req = new SendTaskCardMessageRequest
      {
        ToUser = toUser,
        ToParty = toParty,
        ToTag = toTag,
        AgentId = context.AgentId,
        TaskCard = taskCard
      };
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    #endregion 消息推送

    #region 素材管理

    public async Task<UploadMediaResponse> UploadMediaAsync(string appName, string type, IFormFile file)
    {
      var req = new UploadMediaRequest
      {
        Type = type,
        File = file
      };
      await CheckTokenAsync(appName);
      var context = container.GetContextByName(appName);
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    public async Task<DownloadMediaResponse> DownloadMediaAsync(string appName, string mediaId)
    {
      var req = new DownloadMediaRequest
      {
        MediaId = mediaId
      };
      await CheckTokenAsync(appName);
      var context = container.GetContextByName(appName);
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var res = await client.ExecuteAsync(req, context);
      if (res.Addon != null)
      {
        var (stream, fileName, contentType) = (ValueTuple<byte[], string, MediaTypeHeaderValue>)res.Addon;
        return new DownloadMediaResponse
        {
          Stream = stream,
          FileName = fileName,
          Type = contentType.ToString()
        };
      }
      else
      {
        return new DownloadMediaResponse
        {
          ErrorCode = res.ErrorCode,
          ErrorMessage = res.ErrorMessage,
        };
      }
    }

    #endregion 素材管理

    #region 机器人发消息

    public async Task<WeChatResponse> SendWebhookMarkdownAsync(string appName, string key, string content)
    {
      var req = new SendWebhookMarkdownRequest
      {
        Key = key,
        Content = content
      };
      await CheckTokenAsync(appName);
      var context = container.GetContextByName(appName);
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    public async Task<WeChatResponse> SendWebhookImageAsync(string appName, string key, string imagePath)
    {
      var req = new SendWebhookImageRequest
      {
        Key = key,
        ImagePath = imagePath
      };
      await CheckTokenAsync(appName);
      var context = container.GetContextByName(appName);
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    public async Task<WeChatResponse> SendWebhookNewsAsync(string appName, string key, List<Article> articles)
    {
      var req = new SendWebhookNewsRequest
      {
        Key = key,
        Articles = articles
      };
      await CheckTokenAsync(appName);
      var context = container.GetContextByName(appName);
      if (context == null)
      {
        throw new ArgumentNullException("访问通讯录接口需要设置一个约定的 'contact' 应用，appsecret使用企业微信通讯录助手的appsecrect");
      }
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    #endregion 机器人发消息

#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
    #region 应用管理

    public async Task<WeChatResponse> GetAppInfo(string appName)
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> UpdateAppInfo(string appName, string? homeUrl = null, bool? reportLocationFlag = null, string? logoMediaId = null, string? name = null, string? description = null, string? redirectDomain = null, bool? isReportenter = null)
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> UpdateMenu(string appName)
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> GetMenu(string appName)
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> RemoveMenu(string appName)
    {
      throw new NotImplementedException();
    }

    #endregion 应用管理



    #region 网盘管理

    public async Task<WeChatResponse> CreateDiskSpace()
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> AddSpacePermission(
      string spaceId, string userId,
      Dictionary<string, FilePermission> usersPermission, Dictionary<uint, FilePermission> departmentsPermission)
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> RemoveSpacePermission(
      string spaceId, string userId,
      IEnumerable<string> userIds, IEnumerable<string> departmentIds)
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> UpdateSpaceOptions(
      string spaceId, string userId,
      bool? enableWatermark = null, bool? addMemberOnlyAdmin = null, bool? enableShareUrl = null,
      bool? shareUrlNoApproval = null, FilePermission? shareUrlNoApprovalDefaultPermission = null
      )
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> GetSpaceShareUrl(
      string spaceId, string userId
      )
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> UploadFile(string spaceId, string userId, string folderId, string fileName, string localFilePath)
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> UploadFile(string spaceId, string userId, string folderId, string fileName, FileStream fileStream)
    {
      byte[] bytes;
      using (var memoryStream = new MemoryStream())
      {
        fileStream.CopyTo(memoryStream);
        bytes = memoryStream.ToArray();
      }

      string base64 = Convert.ToBase64String(bytes);

      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> DownloadFile(string userId, string fileId)
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> CreateFile(string spaceId, string userId, string folderId, string fileName, FileType fileType)
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> RenameFile(string userId, string fileId, string fileName)
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> MoveFile(string userId, string folderId, IEnumerable<string> fileIds, bool? replace = null)
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> GetFileInfo(string userId, string fileId)
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> AddFilePermission(
      string userId, string fileId,
      Dictionary<string, FilePermission> usersPermission, Dictionary<uint, FilePermission> departmentsPermission)
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> RemoveFilePermission(
      string userId, string fileId,
      Dictionary<string, FilePermission>? usersPermission = null, Dictionary<uint, FilePermission>? departmentsPermission = null)
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> UpdateFileShareOptions(
      string userId, string fileId,
      FileShareScope scope, FilePermission? permission = null)
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> GetFileShareUrl(string userId, string fileId)
    {
      throw new NotImplementedException();
    }

    #endregion 网盘管理



    #region 日程管理

    public async Task<WeChatResponse> CreateCalendar(string appName)
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> UpdateCalendar(string appName)
    {
      throw new NotImplementedException();
    }
        public async Task<WeChatResponse> GetCalendars(string appName, IEnumerable<string> calendarIds)
        {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> RemoveCalendar(string appName, string calendarId)
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> CreateSchedule(string appName, string calendarId, string creatorId, IEnumerable<string> attendeeIds)
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> UpdateSchedule(string appName, string scheduleId)
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> GetSchedule(string appName, string scheduleId)
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> RemoveSchedule(string appName, string scheduleId)
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> GetSchedules(string appName, string calendarId, int offset = 0, int limit = 500)
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> AddScheduleAttendees(string appName, string scheduleId, IEnumerable<string> attendeeIds)
    {
      throw new NotImplementedException();
    }

    public async Task<WeChatResponse> RemoveScheduleAttendees(string appName, string scheduleId, IEnumerable<string> attendeeIds)
    {
      throw new NotImplementedException();
    }

    #endregion 日程管理
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
  }
}