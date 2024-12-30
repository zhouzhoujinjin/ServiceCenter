namespace PureCode.DataTransfer
{
  public enum VerifyStatus
  {
    /// <summary>
    /// 模板验证成功
    /// </summary>
    Success,

    /// <summary>
    /// 有重复的字段
    /// </summary>
    DuplicatedProperties,

    /// <summary>
    /// 未知的失败
    /// </summary>
    Failed,

    /// <summary>
    /// 模板里有错误，例如 Excel 有行头
    /// </summary>
    TemplateNotSupported
  }

  public static class Format
  {
    public const string Date = "yyyyMMdd";
    public const string DateWithDash = "yyyy-MM-dd";
    public const string YearAndMonth = "yyyyMM";
    public const string YearAndMonthWithDash = "yyyy-MM";
    public const string ExcelDateTime = "EXCEL_DATE";
    public const string EmbedImage = "EMBED_IMAGE";
  }
}