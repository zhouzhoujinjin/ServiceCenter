namespace PureCode.WeChat.Work
{
  public static class Consts
  {
    /// <summary>
    /// 联系人应用
    /// </summary>
    public const string ContactAppName = "contact";

    /// <summary>
    /// 企业邮箱应用
    /// </summary>
    public const string EmailName = "email";

    /// <summary>
    /// 日程应用
    /// </summary>
    public const string CalendarAppName = "calendar";

    /// <summary>
    /// 腾讯会议应用
    /// </summary>
    public const string MeetingAppName = "meeting";

    /// <summary>
    /// 微盘应用
    /// </summary>
    public const string DiskAppName = "disk";

    /// <summary>
    /// 审批应用
    /// </summary>
    public const string ApprovalAppName = "approval";

    /// <summary>
    /// 签到应用
    /// </summary>
    public const string CheckInAppName = "checkin";

    /// <summary>
    /// 汇报应用
    /// </summary>
    public const string ReportAppName = "report";

    /// <summary>
    /// 健康应用
    /// </summary>
    public const string HealthAppName = "health";

    /// <summary>
    /// 企业支付应用
    /// </summary>
    public const string PaymentAppName = "payment";

    /// <summary>
    /// 直播应用
    /// </summary>
    public const string LiveAppName = "live";
  }

  public enum FileType
  {
    Folder = 1,
    File = 3,
    Cell = 4
  }

  public enum FilePermission
  {
    View = 1,
    Edit = 2,
    Preview = 4
  }

  public enum FileShareScope
  {
    SpecialUser = 1,
    InCorp = 2,
    Public = 3
  }
}