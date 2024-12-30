using PureCode.Core.DepartmentFeature;
using PureCode.Core.Models;
using System.Text.Json.Serialization;

namespace Approval.Models
{
  public class DepartmentsAndUsers
  {
    [JsonPropertyName("departments")]
    public List<DepartmentModel> Departments { get; set; }
    [JsonPropertyName("users")]
    public List<UserModel> Users { get; set; }

    public DepartmentsAndUsers()
    {
      Departments = new List<DepartmentModel>();
      Users = new List<UserModel>();
    }
  }
}
