﻿using System.Text.Json.Serialization;

namespace PureCode.Core.Models
{
  /// <summary>
  /// 尺寸
  /// </summary>
  public class Size
  {
    /// <summary>
    /// 宽
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 高
    /// </summary>
    public int Height { get; set; }
  }

  /// <summary>
  /// 系统设置
  /// </summary>
  public class GlobalSettings
  {
    /// <summary>
    /// 站点名称
    /// </summary>
    [JsonPropertyName("siteName")]
    public string? SiteName { get; set; }
    [JsonPropertyName("logoUri")]
    public string? LogoUri { get; set; }

    /// <summary>
    /// 站点根路径
    /// </summary>
    [JsonPropertyName("siteRoot")]
    public string? SiteRoot { get; set; }

    /// <summary>
    /// 默认头像大小
    /// </summary>
    [JsonPropertyName("defaultAvatarSize")]
    public Size DefaultAvatarSize { get; set; } = new Size { Width = 40, Height = 40 };

    /// <summary>
    /// 站点维护开关
    /// </summary>
    [JsonPropertyName("siteUnderMaintenance")]
    public bool SiteUnderMaintenance { get; set; }

    /// <summary>
    /// 备案号
    /// </summary>
    [JsonPropertyName("beiAnNo")]
    public string? BeiAnNo { get; set; }
  }
}