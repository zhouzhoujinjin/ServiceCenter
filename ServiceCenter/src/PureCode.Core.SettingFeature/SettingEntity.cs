namespace PureCode.Core.Entities
{
  public class SettingEntity
  {
    public long Id { get; set; }
    public long InstanceId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string? InstanceType { get; set; }

    public bool IsGlobal => InstanceId == 0;
  }
}