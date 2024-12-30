using PureCode.Core.TreeFeature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PureCode.Core.Models
{
  public class MenuItemModel : ITreeNode<MenuItemModel>
  {
    [JsonPropertyName("id")]
    public ulong? Id { get; set; }

    [JsonPropertyName("type")]
    public MenuItemType Type { get; set; } = MenuItemType.Route;

    [JsonPropertyName("uri")]
    public string Uri { get; set; } = "";

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("iconType")]
    public IconType? IconType { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("childrenInvisible")]
    public bool ChildrenInvisible { get; set; }

    [JsonPropertyName("invisible")]
    public bool Invisible { get; set; }

    [JsonPropertyName("isBlank")]
    public bool IsBlank { get; set; }

    [JsonPropertyName("children")]
    public IEnumerable<MenuItemModel>? Children { get; set; }
  }
}