using System;
using System.IO;
using System.Text;

namespace PureCode.Security
{
  public static class MD5
  {
    public static string Compute(string data)
    {
      if (string.IsNullOrEmpty(data))
      {
        throw new ArgumentNullException(nameof(data));
      }

      using var md5 = System.Security.Cryptography.MD5.Create();
      var hsah = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
      return BitConverter.ToString(hsah).Replace("-", "");
    }

    public static string Compute(Stream stream, bool toUpper = true)
    {
      stream.Position = 0;

      using System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
      byte[] ret = md5.ComputeHash(stream);

      StringBuilder sb = new();
      for (int i = 0; i < ret.Length; i++)
      {
        sb.Append(ret[i].ToString(toUpper ? "X2" : "x2"));
      }

      string md5str = sb.ToString();
      return md5str;
    }
  }
}