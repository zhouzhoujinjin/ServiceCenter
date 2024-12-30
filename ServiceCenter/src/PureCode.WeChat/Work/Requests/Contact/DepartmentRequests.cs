using PureCode.WeChat.Work.Responses;

namespace PureCode.WeChat.Work.Requests
{
  public class CreateDepartmentRequest : WeChatRequest<CreateDepartmentResponse>
  {
    public string DepartmentName { get; set; }
    public string? DepartmentNameEn { get; set; }
    public int? ParentId { get; set; }
    public int? Id { get; set; }
    public int? Order { get; set; }

    public override HttpMethod RequestMethod => HttpMethod.POST;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/department/create";

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
        { "name", DepartmentName },
        { "name_en", DepartmentNameEn },
        { "parentid", ParentId??1 },
        { "order", Order }
      };
      return pp;
    }
  }

  public class UpdateDepartmentRequest : WeChatRequest<WeChatResponse>
  {
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public string? DepartmentNameEn { get; set; }
    public int? ParentId { get; set; }
    public int? Order { get; set; }

    public override HttpMethod RequestMethod => HttpMethod.POST;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/department/update";

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
        {"id", DepartmentId },
      };
      pp.Add("name", DepartmentName);
      pp.Add("name_en", DepartmentNameEn);
      pp.Add("parentid", ParentId);
      pp.Add("order", Order);
      return pp;
    }
  }

  public class ListDepartmentsRequest : WeChatRequest<ListDepartmentsResponse>
  {
    public int? Id { get; set; }
    public override HttpMethod RequestMethod => HttpMethod.GET;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/department/list";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken },
        { "id", Id??1 }
      };
      return qs;
    }
  }

  public class DeleteDepartmentsRequest : WeChatRequest<ListDepartmentsResponse>
  {
    public int Id { get; set; }
    public override HttpMethod RequestMethod => HttpMethod.GET;
    public override string CommandUrl => "https://qyapi.weixin.qq.com/cgi-bin/department/delete";

    public override WeChatDictionary GetQueryString(WeChatContext context)
    {
      var qs = new WeChatDictionary
      {
        { WeChat.WeChatConsts.AccessToken, context.AccessToken },
        { "id", Id }
      };
      return qs;
    }
  }
}