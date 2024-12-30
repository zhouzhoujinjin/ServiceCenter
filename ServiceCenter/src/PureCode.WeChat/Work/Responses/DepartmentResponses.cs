using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PureCode.WeChat.Work.Responses
{
  public class CreateDepartmentResponse : WeChatResponse
  {
    [JsonPropertyName("id")]
    public int Id { get; set; }
  }

  public class Department
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string NameEn { get; set; }
    public int ParentId { get; set; }
    public int Order { get; set; }
  }

  public class ListDepartmentsResponse : WeChatResponse
  {
    [JsonPropertyName("department")]
    public List<Department> Departments { get; set; }
  }
}