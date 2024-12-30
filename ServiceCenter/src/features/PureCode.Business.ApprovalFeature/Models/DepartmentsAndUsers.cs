using PureCode.Core.DepartmentFeature;
using PureCode.Core.Models;
using System.Text.Json.Serialization;

namespace PureCode.Business.ApprovalFeature.Models;

public class DepartmentsAndUsers
{
  [JsonPropertyName("departments")] public List<DepartmentModel> Departments { get; set; } = [];
  [JsonPropertyName("users")] public List<UserModel> Users { get; set; } = [];
}