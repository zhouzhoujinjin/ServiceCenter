using System;

namespace PureCode.DataTransfer.Utils
{
  public static class DateTimeUtils
  {
    private static readonly DateTime Start = new DateTime(1900, 1, 1);

    public static DateTime FromExcelSerialDate(int serialDate)
    {
      if (serialDate > 59) serialDate -= 1; //Excel/Lotus 2/29/1900 bug
      return new DateTime(1899, 12, 31).AddDays(serialDate);
    }

    public static int ToExcelSerialDate(DateTime? date)
    {
      if (date == null)
      {
        return default;
      }
      TimeSpan diff = date.Value - Start;
      return diff.Days + 2;
    }
  }
}