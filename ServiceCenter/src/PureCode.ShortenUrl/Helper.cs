using PureCode.Security;

namespace PureCode.ShortenUrl
{
  internal static class Helper
  {
    public static string CreateToken(long timestamp, string secretKey)
    {
      return MD5.Compute($"{timestamp}{secretKey}").ToLower();
    }
  }
}