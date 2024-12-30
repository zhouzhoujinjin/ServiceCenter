using PureCode.Security;
using PureCode.WeChat.MiniProgram.Requests;
using PureCode.WeChat.MiniProgram.Responses;
using PureCode.WeChat.Requests;
using PureCode.WeChat.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PureCode.WeChat.MiniProgram
{
  public class WeChatMiniProgramApi
  {
    private readonly IWeChatClient client;
    private readonly IWeChatResolver wechatResolver;

    public WeChatMiniProgramApi(IWeChatClient client, IWeChatResolver wechatResolver)
    {
      this.client = client;
      this.wechatResolver = wechatResolver;
    }

    private async Task CheckTokenAsync()
    {
      if (string.IsNullOrEmpty(wechatResolver.Context.AccessToken) || wechatResolver.Context.AccessTokenExpiredTime < DateTime.Now)
      {
        await GetAccessTokenAsync();
      }
    }

    public async Task<AccessTokenResponse> GetAccessTokenAsync()
    {
      var req = new GetAccessTokenRequest();
      var res = await client.ExecuteAsync(req, wechatResolver.Context);
      await wechatResolver.Container.UpdateContextAccessToken(wechatResolver.Context.AppId, res.AccessToken, res.ExpiresIn);
      return res;
    }

    /// <summary>
    /// 使用 code 换取 openId，sessionKey，unionId
    /// <see href="https://developers.weixin.qq.com/miniprogram/dev/api-backend/open-api/login/auth.code2Session.html"/>
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public async Task<CodeToSessionResponse> SessionAsync(string code)
    {
      var req = new CodeToSessionRequest
      {
        Code = code
      };
      await CheckTokenAsync();
      var res = await client.ExecuteAsync(req, wechatResolver.Context);
      await wechatResolver.Container.StoreUserAsync(wechatResolver.Context.AppId, new WeChatUser
      {
        OpenId = res.OpenId,
        UnionId = res.UnionId,
        SessionKey = res.SessionKey
      });
      return res;
    }

    public async Task<bool> CheckUserInfoAsync(string openId, string rawData, string signature)
    {
      var user = await wechatResolver.Container.GetUserAsync(wechatResolver.Context.AppId, openId);
      return signature.ToUpper() == SHA1.Compute(rawData + user.SessionKey);
    }

    /// <summary>
    /// 由加密数据获取 UserInfo
    /// <see href="https://developers.weixin.qq.com/miniprogram/dev/api/open-api/user-info/wx.getUserInfo.html"/>
    /// </summary>
    /// <param name="openId"></param>
    /// <param name="encryptedData"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    public async Task<UserInfoResponse?> GetUserInfoAsync(string openId, string encryptedData, string iv)
    {
      var user = await wechatResolver.Container.GetUserAsync(wechatResolver.Context.AppId, openId);

      var aesIV = Convert.FromBase64String(iv);
      var aesKey = Convert.FromBase64String(user.SessionKey);
      try
      {
        var result = AES.DecryptFromBase64(encryptedData, aesKey, aesIV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
        return JsonSerializer.Deserialize<UserInfoResponse>(result);
      }
      catch
      {
        return null;
      }
    }

    /// <summary>
    /// 由加密数据获取 UserPhone 信息
    /// <see href="https://developers.weixin.qq.com/miniprogram/dev/api/open-api/user-info/wx.getUserInfo.html"/>
    /// </summary>
    /// <param name="openId"></param>
    /// <param name="encryptedData"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    public async Task<UserPhoneResponse?> GetUserPhoneAsync(string openId, string encryptedData, string iv)
    {
      var user = await wechatResolver.Container.GetUserAsync(wechatResolver.Context.AppId, openId);

      var aesIV = Convert.FromBase64String(iv);
      var aesKey = Convert.FromBase64String(user.SessionKey);

      var result = AES.DecryptFromBase64(encryptedData, aesKey, aesIV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
      return JsonSerializer.Deserialize<UserPhoneResponse>(result);
    }

    public async Task<WeChatResponse> SendSubscribeMessage(WeChatContext context, string openId, string templateId, Dictionary<string, string> data)
    {
      var req = new SendSubscribeMessageRequest
      {
        ToUser = openId,
        TemplateId = templateId,
        Data = new SubscribeMessageData(data)
      };
      await CheckTokenAsync();
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    public async Task<WeChatResponse> SendSubscribeMessage(string openId, string templateId, Dictionary<string, string> data)
    {
      var req = new SendSubscribeMessageRequest
      {
        ToUser = openId,
        TemplateId = templateId,
        Data = new SubscribeMessageData(data)
      };
      await CheckTokenAsync();
      var res = await client.ExecuteAsync(req, wechatResolver.Context);
      return res;
    }

    public async Task<WeChatResponse> SendUniformMessageAsync(WeChatContext context, string openId, string formId, string templateId, Dictionary<string, string> data)
    {
      var req = new SendUniformMessageRequest
      {
        ToUser = openId,
        TemplateId = templateId,
        FormId = formId,
        Data = new UniformMessageData(data)
      };
      await CheckTokenAsync();
      var res = await client.ExecuteAsync(req, context);
      return res;
    }

    public async Task<WeChatResponse> SendUniformMessageAsync(string openId, string formId, string templateId, Dictionary<string, string> data)
    {
      var req = new SendUniformMessageRequest
      {
        ToUser = openId,
        TemplateId = templateId,
        FormId = formId,
        Data = new UniformMessageData(data)
      };
      await CheckTokenAsync();
      var res = await client.ExecuteAsync(req, wechatResolver.Context);
      return res;
    }

    public async Task<WeChatResponse> SendUniformMessageAsync(WeChatContext context, string openId, string formId, string templateName, IEnumerable<string> values)
    {
      if (context.TemplateMessageInfos.TryGetValue(templateName, out var templateInfo))
      {
        var keys = templateInfo.Keys;
        var data = keys.Zip(values, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

        var req = new SendUniformMessageRequest
        {
          ToUser = openId,
          TemplateId = templateInfo.TemplateId,
          FormId = formId,
          Data = new UniformMessageData(data)
        };
        await CheckTokenAsync();
        var res = await client.ExecuteAsync(req, context);
        return res;
      }
      else
      {
        return new WeChatResponse { ErrorCode = 47003 };
      }
    }

    public async Task<WeChatResponse> SendUniformMessageAsync(string openId, string formId, string templateName, IEnumerable<string> values)
    {
      if (wechatResolver.Context.TemplateMessageInfos.TryGetValue(templateName, out var templateInfo))
      {
        var keys = templateInfo.Keys;
        var data = keys.Zip(values, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

        var req = new SendUniformMessageRequest
        {
          ToUser = openId,
          TemplateId = templateInfo.TemplateId,
          FormId = formId,
          Data = new UniformMessageData(data)
        };
        await CheckTokenAsync();
        var res = await client.ExecuteAsync(req, wechatResolver.Context);
        return res;
      }
      else
      {
        return new WeChatResponse { ErrorCode = 47003 };
      }
    }

    /// <summary>
    /// 获取小程序二维码
    /// </summary>
    /// <param name="path"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    public Task<Stream> GetQrCode(string path, int width = 430)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 获取默认样式小程序码
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="page"></param>
    /// <param name="width"></param>
    /// <param name="backgroundIsTransparent"></param>
    /// <returns></returns>
    public Task<Stream> GetMiniProgramCode(string scene, string page, int width, bool backgroundIsTransparent)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 获取自定义颜色小程序码
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="page"></param>
    /// <param name="width"></param>
    /// <param name="red"></param>
    /// <param name="blue"></param>
    /// <param name="green"></param>
    /// <returns></returns>
    public Task<Stream> GetMiniProgramCode(string scene, string page, int width, string red, string blue, string green)
    {
      throw new NotImplementedException();
    }
  }
}