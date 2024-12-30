using System;
using System.IO;
using System.Text;

namespace PureCode.Utils
{
  public static class Base64Extensions
  {
    public static string ToBase64(this Stream stream)
    {
      byte[] arr = new byte[stream.Length];
      stream.Position = 0;
      stream.Read(arr, 0, (int)stream.Length);
      return Convert.ToBase64String(arr);
    }

    public static string ToBase64(this string str)
    {
      var plainTextBytes = Encoding.UTF8.GetBytes(str);
      return Convert.ToBase64String(plainTextBytes);
    }
  }
}