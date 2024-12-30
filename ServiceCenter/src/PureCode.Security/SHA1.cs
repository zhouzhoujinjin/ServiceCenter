using System;
using System.Text;

namespace PureCode.Security
{
  public static class SHA1
  {
    public static string Compute(string data)
    {
      if (string.IsNullOrEmpty(data))
      {
        throw new ArgumentNullException(nameof(data));
      }

      using var sha1 = System.Security.Cryptography.SHA1.Create();
      var hsah = sha1.ComputeHash(Encoding.UTF8.GetBytes(data));
      return BitConverter.ToString(hsah).Replace("-", "");
    }
  }
}