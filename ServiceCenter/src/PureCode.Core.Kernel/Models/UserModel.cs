using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PureCode.Core.Models;

public class UserModel
{
  public UserModel()
  {
  }

  public UserModel(ulong id, string userName)
  {
    Id = id;
    UserName = userName;
  }

  [JsonPropertyName("id")] public ulong Id { get; set; }

  [JsonPropertyName("userName")] public string? UserName { get; set; }

  [JsonPropertyName("profiles")] public Dictionary<string, object?> Profiles { get; set; } = [];

  [JsonPropertyName("roles")] public Dictionary<string, string>? Roles { get; set; }
}

public class AdminUserModel : UserModel
{
  public bool IsDeleted { get; set; }

  public bool IsVisible { get; set; }

  public ulong? CreatorId { get; set; }
  public string? CreatorName { get; set; }
  public string? FullName { get; set; }

  public static AdminUserModel FromUser(UserModel user)
  {
    return new AdminUserModel
    {
      Id = user.Id,
      UserName = user.UserName,
      Profiles = user.Profiles
    };
  }
}