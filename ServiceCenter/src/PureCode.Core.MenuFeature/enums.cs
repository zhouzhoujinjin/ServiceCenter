namespace PureCode.Core
{
  public enum MenuItemType
  {
    /// <summary>
    /// 菜单路由项
    /// </summary>
    Route = 1,

    /// <summary>
    /// 菜单权限项
    /// </summary>
    Action = 2,

    /// <summary>
    /// 外部链接
    /// </summary>
    Link = 3,

    /// <summary>
    /// 分组
    /// </summary>
    Group = 4
  }

  public enum IconType
  {
    Builtin,
    Image,
  }
}