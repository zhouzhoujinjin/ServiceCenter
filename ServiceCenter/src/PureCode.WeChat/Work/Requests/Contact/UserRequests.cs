using PureCode.WeChat.Work.Responses;
using System.Collections.Generic;

namespace PureCode.WeChat.Work.Requests
{
  //创建成员
  public class CreateUserRequest : WeChatRequest<WeChatResponse>
  {
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string UserAlias { get; set; }
    public string Mobile { get; set; }
    public ICollection<int>? Department { get; set; }
    public ICollection<int>? Order { get; set; }
    public string Position { get; set; }
    public string Gender { get; set; }
    public string Email { get; set; }
    public ICollection<int> IsLeaderInDept { get; set; }
    public int? Enable { get; set; }
    public string AvatarMediaId { get; set; }
    public string Telephone { get; set; }
    public string Address { get; set; }
    public int? MainDepartment { get; set; }
    public bool? ToInvite { get; set; }
    public string ExternalPosition { get; set; }

    public override HttpMethod RequestMethod => HttpMethod.POST;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/user/create";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken }
      };
      return qs;
    }

    public override WeChatDictionary GetPostParameters()
    {
      var pp = new WeChatDictionary
      {
        { "userid", UserId },
        { "name", UserName },
        { "alias", UserAlias },
        { "mobile", Mobile },
        { "department", Department ?? new List<int> { 1 } },
        { "order", Order },
        { "position", Position },
        { "gender", Gender },
        { "email", Email },
        { "is_leader_in_dept", IsLeaderInDept },
        { "enable", Enable?? 1 },
        { "avatar_mediaid", AvatarMediaId },
        { "telephone", Telephone },
        { "address", Address },
        { "main_department", MainDepartment??1 },
        { "to_invite", ToInvite??true },
        { "external_position", ExternalPosition }
      };
      return pp;
    }
  }

  //读取成员
  public class GetUserRequest : WeChatRequest<GetUserResponse>
  {
    public string UserId { get; set; }
    public override HttpMethod RequestMethod => HttpMethod.GET;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/user/get";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken },
        { "userid", UserId }
      };
      return qs;
    }
  }

  //修改成员
  public class UpdateUserRequest : WeChatRequest<WeChatResponse>
  {
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string UserAlias { get; set; }
    public string Mobile { get; set; }
    public ICollection<int> Department { get; set; }
    public ICollection<int> Order { get; set; }
    public string Position { get; set; }
    public string Gender { get; set; }
    public string Email { get; set; }
    public ICollection<int> IsLeaderInDept { get; set; }
    public int? Enable { get; set; }
    public string AvatarMediaId { get; set; }
    public string Telephone { get; set; }
    public string Address { get; set; }
    public int? MainDepartment { get; set; }
    public bool? ToInvite { get; set; }
    public string ExternalPosition { get; set; }

    public override HttpMethod RequestMethod => HttpMethod.POST;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/user/update";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken }
      };
      return qs;
    }

    public override WeChatDictionary GetPostParameters()
    {
      var pp = new WeChatDictionary
      {
        { "userid", UserId },
        { "name", UserName },
        { "alias", UserAlias },
        { "mobile", Mobile },
        { "department", Department ?? new List<int> { 1 } },
        { "order", Order },
        { "position", Position },
        { "gender", Gender },
        { "email", Email },
        { "is_leader_in_dept", IsLeaderInDept },
        { "enable", Enable?? 1 },
        { "avatar_mediaid", AvatarMediaId },
        { "telephone", Telephone },
        { "address", Address },
        { "main_department", MainDepartment??1 },
        { "to_invite", ToInvite??true },
        { "external_position", ExternalPosition }
      };
      return pp;
    }
  }

  //删除成员
  public class DeleteUserRequest : WeChatRequest<WeChatResponse>
  {
    public string UserId { get; set; }
    public override HttpMethod RequestMethod => HttpMethod.GET;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/user/delete";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken },
        { "userid", UserId }
      };
      return qs;
    }
  }

  //批量删除成员
  public class BatchDeleteUserRequest : WeChatRequest<WeChatResponse>
  {
    public ICollection<string> UserIdList { get; set; }
    public override HttpMethod RequestMethod => HttpMethod.POST;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/user/batchdelete";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken }
      };
      return qs;
    }

    public override WeChatDictionary GetPostParameters()
    {
      var pp = new WeChatDictionary
      {
        { "useridlist", UserIdList }
      };
      return pp;
    }
  }

  //获取部门成员
  public class GetDepartmentUsersRequest : WeChatRequest<UserListResponse>
  {
    public int DepartmentId { get; set; }
    public int? FetchChild { get; set; }
    public override HttpMethod RequestMethod => HttpMethod.GET;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/user/simplelist";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken },
        { "department_id", DepartmentId},
        { "fetch_child", FetchChild ?? 1 }
      };
      return qs;
    }
  }

  //获取部门成员详情
  public class GetDepartmentUsersDetailRequest : WeChatRequest<UserDetailListResponse>
  {
    public int DepartmentId { get; set; }
    public int? FetchChild { get; set; }
    public override HttpMethod RequestMethod => HttpMethod.GET;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/user/list";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken },
        { "department_id", DepartmentId},
        { "fetch_child", FetchChild ?? 1 }
      };
      return qs;
    }
  }

  //userid转openid
  public class ConvertOpenIdRequest : WeChatRequest<ConvertOpenIdResponse>
  {
    public string UserId { get; set; }
    public override HttpMethod RequestMethod => HttpMethod.POST;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/user/convert_to_openid";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken }
      };
      return qs;
    }

    public override WeChatDictionary GetPostParameters()
    {
      var pp = new WeChatDictionary
      {
        { "userid", UserId }
      };
      return pp;
    }
  }

  //openid转userid
  public class ConvertUserIdRequest : WeChatRequest<ConvertUserIdResponse>
  {
    public string OpenId { get; set; }
    public override HttpMethod RequestMethod => HttpMethod.POST;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/user/convert_to_userid";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken }
      };
      return qs;
    }

    public override WeChatDictionary GetPostParameters()
    {
      var pp = new WeChatDictionary
      {
        { "openid", OpenId }
      };
      return pp;
    }
  }

  //二次验证
  public class AuthsuccRequest : WeChatRequest<WeChatResponse>
  {
    public string UserId { get; set; }
    public override HttpMethod RequestMethod => HttpMethod.GET;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/user/authsucc";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken },
        { "userid", UserId }
      };
      return qs;
    }
  }

  //邀请成员
  public class InviteUserOrDepartmentRequest : WeChatRequest<InviteResponse>
  {
    public ICollection<string> UserIds { get; set; }
    public ICollection<int> DepartmentIds { get; set; }
    public ICollection<int> Tags { get; set; }

    public override HttpMethod RequestMethod => HttpMethod.POST;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/batch/invite";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken }
      };
      return qs;
    }

    public override WeChatDictionary GetPostParameters()
    {
      var pp = new WeChatDictionary
      {
        { "user", UserIds },
        { "party", DepartmentIds },
        { "tag", Tags }
      };
      return pp;
    }
  }

  //获取加入企业二维码
  public class GetJoinQrcodeRequest : WeChatRequest<GetJoinQrcodeResponse>
  {
    //1: 171 x 171; 2: 399 x 399; 3: 741 x 741; 4: 2052 x 2052
    public int? SizeType { get; set; } = 1;

    public override HttpMethod RequestMethod => HttpMethod.GET;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/corp/get_join_qrcode";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken },
        { "size_type", SizeType ?? 1 }
      };
      return qs;
    }
  }

  //获取企业活跃成员数
  public class GetActiveStatRequest : WeChatRequest<GetActiveStatResponse>
  {
    public string Date { get; set; }
    public override HttpMethod RequestMethod => HttpMethod.POST;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/user/get_active_stat";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken },
      };
      return qs;
    }

    public override WeChatDictionary GetPostParameters()
    {
      var pp = new WeChatDictionary
      {
        { "date", Date }
      };
      return pp;
    }
  }
}