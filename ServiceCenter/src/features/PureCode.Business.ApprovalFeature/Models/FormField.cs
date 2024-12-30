using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PureCode.Business.ApprovalFeature.Models;

public class FormField
{
  [JsonPropertyName("title")] public string? Title { get; set; }

  [JsonPropertyName("name")] public string Name { get; set; } = null!;
  [JsonPropertyName("valueType")] public string ValueType { get; set; } = null!;
  [JsonPropertyName("controlType")] public string ControlType { get; set; } = null!;
  [JsonPropertyName("required")] public bool? Required { get; set; }

  [JsonPropertyName("controlOptions")] public Dictionary<string, object> ControlOptions { get; set; } = [];
}

public class BlockField : FormField
{
  [JsonPropertyName("minCount")] public int? MinCount { get; set; }
  [JsonPropertyName("maxCount")] public int? MaxCount { get; set; }

  [JsonPropertyName("children")] public List<FormField> Children { get; set; } = [];
}

public class ReturnTimeFormField
{
  [JsonPropertyName("returnTime")] public DateTime ReturnTime { get; set; }
}

public class OvertimeFinishDate
{
  public int ItemId { get; set; }
  public DateTime FinishDate { get; set; }
}