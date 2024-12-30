using PureCode.Core.Models;
using PureCode.Core.TreeFeature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PureCode.Core.DepartmentFeature
{
  public class DepartmentModel: ITreeNode<DepartmentModel>
  {
    [JsonPropertyName("id")]
    public ulong? Id { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    [JsonPropertyName("creator")]
    public UserModel? Creator { get; set; }
    [JsonPropertyName("createdTime")]
    public DateTime? CreatedTime { get; set; }
    [JsonPropertyName("users")]
    public IEnumerable<DepartmentUserModel>? Users { get; set; }
    public IEnumerable<DepartmentModel>? Children { get; set; }
  }

  public class DepartmentUserModel : UserModel
  {

    [JsonPropertyName("userId")]
    public ulong UserId { get; set; }

    [JsonPropertyName("position")]
    public string? Position { get; set; }

    [JsonPropertyName("level")]
    public short? Level { get; set; }

    [JsonPropertyName("isUserMajorDepartment")]
    public bool? IsUserMajorDepartment { get; set; }

    [JsonPropertyName("departments")]
    public IEnumerable<UserDepartment>? Departments { get; set; }

    public static DepartmentUserModel FromUser(UserModel user)
    {
      var du = new DepartmentUserModel
      {
        Id = user.Id,
        Profiles = user.Profiles,
        Roles = user.Roles,
        UserName = user.UserName
      };
      return du;
    }
  }

  public class UserDepartment
  {
    [JsonPropertyName("departmentId")]
    public ulong DepartmentId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("position")]
    public string? Position { get; set; }

    [JsonPropertyName("level")]
    public short? Level { get; set; }

    [JsonPropertyName("isUserMajorDepartment")]
    public bool? IsUserMajorDepartment { get; set; }
  }

}
