using System.ComponentModel.DataAnnotations.Schema;

namespace PureCode.Core.Entities
{
  public class ValueSpaceEntity
  {
    public ulong Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;

    public ConfigureLevel ConfigureLevel { get; set; }
    public ValueSpaceType ValueSpaceType { get; set; }
    public string Items { get; set; } = string.Empty;

    public object? ItemList { get; set; }
  }
}