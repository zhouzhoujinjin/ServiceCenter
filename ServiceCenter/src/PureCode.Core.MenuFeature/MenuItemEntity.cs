namespace PureCode.Core.Entities
{
  public class MenuItemEntity : IEntity
  {
    public ulong Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Uri { get; set; } = string.Empty;
    public MenuItemType Type { get; set; }
    public IconType? IconType { get; set; }
    public string? Icon { get; set; } = string.Empty;
    public bool IsBlank { get; set; } = false;
  }
}