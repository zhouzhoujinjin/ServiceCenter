using System.Text.Json.Serialization;

namespace Approval.Models
{
  public class DepartmentUserSelect
  {
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("value")]
    public ulong Value { get; set; }

    [JsonPropertyName("avatar")]
    public string Avatar { get; set; }

    [JsonPropertyName("children")]
    public ICollection<DepartmentUserSelect> Children { get; set; }

    public DepartmentUserSelect()
    {
      Children = new List<DepartmentUserSelect>();
    }
  }
}