using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PureCode.WeChat.Work.Responses
{
  public class GetUserResponse : WeChatResponse
  {
    [JsonPropertyName("userid")]
    public string UserId { get; set; }

    [JsonPropertyName("name")]
    public string UserName { get; set; }

    [JsonPropertyName("department")]
    public ICollection<int> Department { get; set; }

    [JsonPropertyName("order")]
    public ICollection<int> Order { get; set; }

    [JsonPropertyName("position")]
    public string Position { get; set; }

    [JsonPropertyName("mobile")]
    public string Mobile { get; set; }

    [JsonPropertyName("gender")]
    public string Gender { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("is_leader_in_dept")]
    public ICollection<int> IsLeaderInDept { get; set; }

    [JsonPropertyName("avatar")]
    public string Avatar { get; set; }

    [JsonPropertyName("thumb_avatar")]
    public string ThumbAvatar { get; set; }

    [JsonPropertyName("telephone")]
    public string Telephone { get; set; }

    [JsonPropertyName("alias")]
    public string UserAlias { get; set; }

    [JsonPropertyName("address")]
    public string Address { get; set; }

    [JsonPropertyName("main_department")]
    public int? MainDepartment { get; set; }

    [JsonPropertyName("open_userid")]
    public string OpenUserId { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("qr_code")]
    public string QrCode { get; set; }

    [JsonPropertyName("external_position")]
    public string ExternalPosition { get; set; }
  }

  public class UserInfo
  {
    [JsonPropertyName("userid")]
    public string UserId { get; set; }

    [JsonPropertyName("name")]
    public string UserName { get; set; }

    [JsonPropertyName("department")]
    public ICollection<int> Department { get; set; }

    [JsonPropertyName("open_userid")]
    public string OpenUserId { get; set; }
  }

  public class UserListResponse : WeChatResponse
  {
    [JsonPropertyName("userlist")]
    public ICollection<UserInfo> UserList { get; set; }
  }

  public class UserInfoDetail : UserInfo
  {
    [JsonPropertyName("order")]
    public ICollection<int> Order { get; set; }

    [JsonPropertyName("position")]
    public string Position { get; set; }

    [JsonPropertyName("mobile")]
    public string Mobile { get; set; }

    [JsonPropertyName("gender")]
    public string Gender { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("is_leader_in_dept")]
    public ICollection<int> IsLeaderInDept { get; set; }

    [JsonPropertyName("avatar")]
    public string Avatar { get; set; }

    [JsonPropertyName("thumb_avatar")]
    public string ThumbAvatar { get; set; }

    [JsonPropertyName("telephone")]
    public string Telephone { get; set; }

    [JsonPropertyName("alias")]
    public string UserAlias { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("address")]
    public string Address { get; set; }

    [JsonPropertyName("hide_mobile")]
    public int HideMobile { get; set; }

    [JsonPropertyName("english_name")]
    public string EnglishName { get; set; }

    [JsonPropertyName("main_department")]
    public int? MainDepartment { get; set; }

    [JsonPropertyName("qr_code")]
    public string QrCode { get; set; }

    [JsonPropertyName("external_position")]
    public string ExternalPosition { get; set; }
  }

  public class UserDetailListResponse : WeChatResponse
  {
    [JsonPropertyName("userlist")]
    public ICollection<UserInfoDetail> UserList { get; set; }
  }

  public class ConvertOpenIdResponse : WeChatResponse
  {
    [JsonPropertyName("openid")]
    public string OpenId { get; set; }
  }

  public class ConvertUserIdResponse : WeChatResponse
  {
    [JsonPropertyName("userid")]
    public string UserId { get; set; }
  }

  public class InviteResponse : WeChatResponse
  {
    [JsonPropertyName("invaliduser")]
    public ICollection<string> InvalidUsers { get; set; }

    [JsonPropertyName("invalidparty")]
    public ICollection<int> InvalidParty { get; set; }

    [JsonPropertyName("invalidtag")]
    public ICollection<int> InvalidTag { get; set; }
  }

  public class GetJoinQrcodeResponse : WeChatResponse
  {
    [JsonPropertyName("join_qrcode")]
    public string JoinQrcodeUrl { get; set; }
  }

  public class GetActiveStatResponse : WeChatResponse
  {
    [JsonPropertyName("active_cnt")]
    public int ActiveCnt { get; set; }
  }
}