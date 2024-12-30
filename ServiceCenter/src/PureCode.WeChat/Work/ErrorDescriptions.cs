using System.ComponentModel;

namespace PureCode.WeChat.Work
{
  public enum ErrorDescriptions
  {
    [Description("系统繁忙")]
    SystemBusy = -1,

    [Description("请求成功")]
    Ok = 0,

    [Description("数据版本冲突")]
    DataVersionConflict = 6000,
  }
}